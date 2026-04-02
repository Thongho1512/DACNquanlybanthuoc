using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using quanlybanthuoc.Data.Entities;
using quanlybanthuoc.Data.Repositories;
using quanlybanthuoc.Dtos.DonNhapHang;
using quanlybanthuoc.Middleware.Exceptions;
using quanlybanthuoc.Services.Impl;
using quanlybanthuoc.Dtos;

namespace quanlybanthuoc.Tests.Services
{
    public class DonNhapHangServiceCFGTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<DonNhapHangService>> _loggerMock;
        
        // Repositories
        private readonly Mock<IDonNhapHangRepository> _donNhapHangRepoMock;
        private readonly Mock<IChiNhanhRepository> _chiNhanhRepoMock;
        private readonly Mock<INhaCungCapRepository> _nhaCungCapRepoMock;
        private readonly Mock<IThuocRepository> _thuocRepoMock;
        private readonly Mock<ILoHangRepository> _loHangRepoMock;
        private readonly Mock<IKhoHangRepository> _khoHangRepoMock;
        
        private readonly DonNhapHangService _service;

        public DonNhapHangServiceCFGTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<DonNhapHangService>>();
            
            _donNhapHangRepoMock = new Mock<IDonNhapHangRepository>();
            _chiNhanhRepoMock = new Mock<IChiNhanhRepository>();
            _nhaCungCapRepoMock = new Mock<INhaCungCapRepository>();
            _thuocRepoMock = new Mock<IThuocRepository>();
            _loHangRepoMock = new Mock<ILoHangRepository>();
            _khoHangRepoMock = new Mock<IKhoHangRepository>();

            _unitOfWorkMock.Setup(u => u.DonNhapHangRepository).Returns(_donNhapHangRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.ChiNhanhRepository).Returns(_chiNhanhRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.NhaCungCapRepository).Returns(_nhaCungCapRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.ThuocRepository).Returns(_thuocRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.LoHangRepository).Returns(_loHangRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.KhoHangRepository).Returns(_khoHangRepoMock.Object);

            _service = new DonNhapHangService(
                _unitOfWorkMock.Object,
                _loggerMock.Object,
                _mapperMock.Object
            );
        }

        #region --- EXCEPTION PATHS ---

        [Fact]
        public async Task TC1_CreateAsync_BranchNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var dto = new CreateDonNhapHangDto { IdchiNhanh = 999 };
            _chiNhanhRepoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((ChiNhanh)null);

            // Act & Assert
            await _service.Invoking(s => s.CreateAsync(dto, 1))
                .Should().ThrowAsync<NotFoundException>()
                .WithMessage("Chi nhánh không tồn tại hoặc không hoạt động.");
        }

        [Fact]
        public async Task TC2_CreateAsync_SupplierNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var dto = new CreateDonNhapHangDto { IdchiNhanh = 1, IdnhaCungCap = 999 };
            _chiNhanhRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new ChiNhanh { Id = 1, TrangThai = true });
            _nhaCungCapRepoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((NhaCungCap)null);

            // Act & Assert
            await _service.Invoking(s => s.CreateAsync(dto, 1))
                .Should().ThrowAsync<NotFoundException>()
                .WithMessage("Nhà cung cấp không tồn tại.");
        }

        [Fact]
        public async Task TC3_CreateAsync_DuplicateInvoiceCode_ThrowsBadRequestException()
        {
            // Arrange
            var dto = new CreateDonNhapHangDto { IdchiNhanh = 1, IdnhaCungCap = 1, SoDonNhap = "IMPORT01" };
            _chiNhanhRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new ChiNhanh { Id = 1, TrangThai = true });
            _nhaCungCapRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new NhaCungCap { Id = 1, TrangThai = true });
            _donNhapHangRepoMock.Setup(r => r.GetBySoDonNhapAsync("IMPORT01")).ReturnsAsync(new DonNhapHang());

            // Act & Assert
            await _service.Invoking(s => s.CreateAsync(dto, 1))
                .Should().ThrowAsync<BadRequestException>()
                .WithMessage("Số đơn nhập đã tồn tại.");
        }

        [Fact]
        public async Task TC4_CreateAsync_MedicineNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var dto = new CreateDonNhapHangDto 
            { 
                IdchiNhanh = 1, 
                IdnhaCungCap = 1, 
                SoDonNhap = "NEW",
                LoHangs = new List<LoHangNhapDto> { new LoHangNhapDto { Idthuoc = 999 } }
            };

            _chiNhanhRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new ChiNhanh { Id = 1, TrangThai = true });
            _nhaCungCapRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new NhaCungCap { Id = 1, TrangThai = true });
            _donNhapHangRepoMock.Setup(r => r.GetBySoDonNhapAsync("NEW")).ReturnsAsync((DonNhapHang)null);
            _thuocRepoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Thuoc)null);

            // Act & Assert
            await _service.Invoking(s => s.CreateAsync(dto, 1))
                .Should().ThrowAsync<NotFoundException>()
                .WithMessage("Thuốc ID 999 không tồn tại.");
        }

        [Fact]
        public async Task TC5_CreateAsync_InvalidExpiryDate_ThrowsBadRequestException()
        {
            // Arrange
            var dto = new CreateDonNhapHangDto 
            { 
                IdchiNhanh = 1, IdnhaCungCap = 1, SoDonNhap = "NEW",
                LoHangs = new List<LoHangNhapDto> 
                { 
                    new LoHangNhapDto 
                    { 
                        Idthuoc = 1, 
                        NgaySanXuat = new DateOnly(2024, 1, 2), 
                        NgayHetHan = new DateOnly(2024, 1, 1) 
                    } 
                }
            };

            _chiNhanhRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new ChiNhanh { Id = 1, TrangThai = true });
            _nhaCungCapRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new NhaCungCap { Id = 1, TrangThai = true });
            _thuocRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Thuoc { Id = 1, TrangThai = true });

            // Act & Assert
            await _service.Invoking(s => s.CreateAsync(dto, 1))
                .Should().ThrowAsync<BadRequestException>()
                .WithMessage("Ngày hết hạn phải sau ngày sản xuất.");
        }

        [Fact]
        public async Task TC6_CreateAsync_ExpiredMedicine_ThrowsBadRequestException()
        {
            // Arrange
            var dto = new CreateDonNhapHangDto 
            { 
                IdchiNhanh = 1, IdnhaCungCap = 1, SoDonNhap = "NEW",
                LoHangs = new List<LoHangNhapDto> 
                { 
                    new LoHangNhapDto 
                    { 
                        Idthuoc = 1, 
                        NgaySanXuat = new DateOnly(2020, 1, 1), 
                        NgayHetHan = new DateOnly(2022, 1, 1) // In the past
                    } 
                }
            };

            _chiNhanhRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new ChiNhanh { Id = 1, TrangThai = true });
            _nhaCungCapRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new NhaCungCap { Id = 1, TrangThai = true });
            _thuocRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Thuoc { Id = 1, TrangThai = true });

            // Act & Assert
            await _service.Invoking(s => s.CreateAsync(dto, 1))
                .Should().ThrowAsync<BadRequestException>()
                .WithMessage("Không thể nhập thuốc đã hết hạn.");
        }

        #endregion

        #region --- SUCCESS PATHS ---

        [Fact]
        public async Task TC7_CreateAsync_Success_CreateNewStock()
        {
            // Arrange
            var dto = new CreateDonNhapHangDto 
            { 
                IdchiNhanh = 1, IdnhaCungCap = 1, SoDonNhap = "NEW_GOOD",
                LoHangs = new List<LoHangNhapDto> 
                { 
                    new LoHangNhapDto { Idthuoc = 1, SoLo = "LOT1", SoLuong = 100, GiaNhap = 5000, NgaySanXuat = new DateOnly(2024, 1, 1), NgayHetHan = new DateOnly(2030, 1, 1) } 
                }
            };

            _chiNhanhRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new ChiNhanh { Id = 1, TrangThai = true });
            _nhaCungCapRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new NhaCungCap { Id = 1, TrangThai = true });
            _thuocRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Thuoc { Id = 1, TrangThai = true });
            _khoHangRepoMock.Setup(r => r.GetByChiNhanhAndLoHangAsync(1, It.IsAny<int>())).ReturnsAsync((KhoHang)null); // New stock
            _donNhapHangRepoMock.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<int>())).ReturnsAsync(new DonNhapHang { Id = 1 });

            // Act
            var result = await _service.CreateAsync(dto, 1);

            // Assert
            _khoHangRepoMock.Verify(r => r.CreateAsync(It.Is<KhoHang>(kh => kh.SoLuongTon == 100)), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task TC8_CreateAsync_Success_UpdateExistingStock()
        {
            // Arrange
            var dto = new CreateDonNhapHangDto 
            { 
                IdchiNhanh = 1, IdnhaCungCap = 1, SoDonNhap = "RE_IMPORT",
                LoHangs = new List<LoHangNhapDto> 
                { 
                    new LoHangNhapDto { Idthuoc = 1, SoLo = "LOT1", SoLuong = 50, GiaNhap = 5000, NgaySanXuat = new DateOnly(2024, 1, 1), NgayHetHan = new DateOnly(2030, 1, 1) } 
                }
            };

            var existingStock = new KhoHang { Id = 10, SoLuongTon = 100 };

            _chiNhanhRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new ChiNhanh { Id = 1, TrangThai = true });
            _nhaCungCapRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new NhaCungCap { Id = 1, TrangThai = true });
            _thuocRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Thuoc { Id = 1, TrangThai = true });
            _khoHangRepoMock.Setup(r => r.GetByChiNhanhAndLoHangAsync(1, It.IsAny<int>())).ReturnsAsync(existingStock);
            _donNhapHangRepoMock.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<int>())).ReturnsAsync(new DonNhapHang { Id = 1 });

            // Act
            await _service.CreateAsync(dto, 1);

            // Assert
            existingStock.SoLuongTon.Should().Be(150); // 100 + 50
            _khoHangRepoMock.Verify(r => r.UpdateAsync(existingStock), Times.Once);
        }

        [Fact]
        public async Task TC9_CreateAsync_Success_MultipleBatches()
        {
            // Arrange
            var dto = new CreateDonNhapHangDto 
            { 
                IdchiNhanh = 1, IdnhaCungCap = 1, SoDonNhap = "MULTIPLE",
                LoHangs = new List<LoHangNhapDto> 
                { 
                    new LoHangNhapDto { Idthuoc = 1, SoLo = "L1", SoLuong = 10, GiaNhap = 1000, NgaySanXuat = new DateOnly(2024, 1, 1), NgayHetHan = new DateOnly(2030, 1, 1) },
                    new LoHangNhapDto { Idthuoc = 2, SoLo = "L2", SoLuong = 20, GiaNhap = 2000, NgaySanXuat = new DateOnly(2024, 1, 1), NgayHetHan = new DateOnly(2030, 1, 1) }
                }
            };

            _chiNhanhRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new ChiNhanh { Id = 1, TrangThai = true });
            _nhaCungCapRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new NhaCungCap { Id = 1, TrangThai = true });
            _thuocRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new Thuoc { TrangThai = true });
            _donNhapHangRepoMock.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<int>())).ReturnsAsync(new DonNhapHang { Id = 1 });

            // Act
            await _service.CreateAsync(dto, 1);

            // Assert
            _donNhapHangRepoMock.Verify(r => r.UpdateAsync(It.Is<DonNhapHang>(d => d.TongTien == 50000)), Times.Once); // 10*1000 + 20*2000 = 50k
        }

        [Fact]
        public async Task TC10_CreateAsync_UnexpectedError_RollsBackAndThrows()
        {
            // Arrange (Path 11: catch block)
            var dto = new CreateDonNhapHangDto { IdchiNhanh = 1, IdnhaCungCap = 1, SoDonNhap = "FAIL" };
            _chiNhanhRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new ChiNhanh { Id = 1, TrangThai = true });
            _nhaCungCapRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new NhaCungCap { Id = 1, TrangThai = true });
            _donNhapHangRepoMock.Setup(r => r.GetBySoDonNhapAsync("FAIL")).ReturnsAsync((DonNhapHang)null);

            // Mock exception during transaction
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ThrowsAsync(new Exception("DB Error"));

            // Act & Assert
            await _service.Invoking(s => s.CreateAsync(dto, 1))
                .Should().ThrowAsync<Exception>()
                .WithMessage("DB Error");

            _unitOfWorkMock.Verify(u => u.RollbackTransactionAsync(), Times.Once);
        }

        #endregion
    }
}
