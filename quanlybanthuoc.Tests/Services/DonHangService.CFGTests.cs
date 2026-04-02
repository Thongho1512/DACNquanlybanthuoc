using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using quanlybanthuoc.Data.Entities;
using quanlybanthuoc.Data.Repositories;
using quanlybanthuoc.Dtos.DonHang;
using quanlybanthuoc.Middleware.Exceptions;
using quanlybanthuoc.Services;
using quanlybanthuoc.Services.Impl;
using quanlybanthuoc.Dtos;

namespace quanlybanthuoc.Tests.Services
{
    public class DonHangServiceCFGTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<DonHangService>> _loggerMock;
        private readonly Mock<IKhachHangService> _khachHangServiceMock;
        
        // Repositories
        private readonly Mock<IDonHangRepository> _donHangRepoMock;
        private readonly Mock<IChiNhanhRepository> _chiNhanhRepoMock;
        private readonly Mock<IKhachHangRepository> _khachHangRepoMock;
        private readonly Mock<IPhuongThucThanhToanRepository> _ptttRepoMock;
        private readonly Mock<IThuocRepository> _thuocRepoMock;
        private readonly Mock<ILoHangRepository> _loHangRepoMock;
        private readonly Mock<IKhoHangRepository> _khoHangRepoMock;
        private readonly Mock<IChiTietDonHangRepository> _ctdhRepoMock;
        private readonly Mock<ILichSuDiemRepository> _lsdiemRepoMock;
        
        private readonly DonHangService _service;

        public DonHangServiceCFGTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<DonHangService>>();
            _khachHangServiceMock = new Mock<IKhachHangService>();
            
            _donHangRepoMock = new Mock<IDonHangRepository>();
            _chiNhanhRepoMock = new Mock<IChiNhanhRepository>();
            _khachHangRepoMock = new Mock<IKhachHangRepository>();
            _ptttRepoMock = new Mock<IPhuongThucThanhToanRepository>();
            _thuocRepoMock = new Mock<IThuocRepository>();
            _loHangRepoMock = new Mock<ILoHangRepository>();
            _khoHangRepoMock = new Mock<IKhoHangRepository>();
            _ctdhRepoMock = new Mock<IChiTietDonHangRepository>();
            _lsdiemRepoMock = new Mock<ILichSuDiemRepository>();

            // Setup UnitOfWork to return mocks
            _unitOfWorkMock.Setup(u => u.DonHangRepository).Returns(_donHangRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.ChiNhanhRepository).Returns(_chiNhanhRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.KhachHangRepository).Returns(_khachHangRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.PhuongThucThanhToanRepository).Returns(_ptttRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.ThuocRepository).Returns(_thuocRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.LoHangRepository).Returns(_loHangRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.KhoHangRepository).Returns(_khoHangRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.ChiTietDonHangRepository).Returns(_ctdhRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.LichSuDiemRepository).Returns(_lsdiemRepoMock.Object);

            _service = new DonHangService(
                _unitOfWorkMock.Object,
                _loggerMock.Object,
                _mapperMock.Object,
                _khachHangServiceMock.Object
            );
        }

        #region --- EXCEPTION PATHS ---

        [Fact]
        public async Task TC1_CreateAsync_BranchNotFound_ThrowsNotFoundException()
        {
            // Arrange (D1: chiNhanh == null)
            var dto = new CreateDonHangDto { IdchiNhanh = 999 };
            _chiNhanhRepoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((ChiNhanh)null);

            // Act & Assert
            await _service.Invoking(s => s.CreateAsync(dto, 1))
                .Should().ThrowAsync<NotFoundException>()
                .WithMessage("Chi nhánh không tồn tại hoặc không hoạt động.");
        }

        [Fact]
        public async Task TC1_CreateAsync_BranchInactive_ThrowsNotFoundException()
        {
            // Arrange (D1: chiNhanh.TrangThai == false)
            var dto = new CreateDonHangDto { IdchiNhanh = 101 };
            _chiNhanhRepoMock.Setup(r => r.GetByIdAsync(101)).ReturnsAsync(new ChiNhanh { Id = 101, TrangThai = false });

            // Act & Assert
            await _service.Invoking(s => s.CreateAsync(dto, 1))
                .Should().ThrowAsync<NotFoundException>()
                .WithMessage("Chi nhánh không tồn tại hoặc không hoạt động.");
        }

        [Fact]
        public async Task TC2_CreateAsync_CustomerProvidedButNotFound_ThrowsNotFoundException()
        {
            // Arrange (D3: khachHang == null)
            var dto = new CreateDonHangDto { IdchiNhanh = 1, IdkhachHang = 999 };
            _chiNhanhRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new ChiNhanh { Id = 1, TrangThai = true });
            _khachHangRepoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((KhachHang)null);

            // Act & Assert
            await _service.Invoking(s => s.CreateAsync(dto, 1))
                .Should().ThrowAsync<NotFoundException>()
                .WithMessage("Khách hàng không tồn tại.");
        }

        [Fact]
        public async Task TC3_CreateAsync_PaymentMethodNotFound_ThrowsNotFoundException()
        {
            // Arrange (D4: phuongThucTt == null)
            var dto = new CreateDonHangDto { IdchiNhanh = 1, IdphuongThucTt = 999 };
            _chiNhanhRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new ChiNhanh { Id = 1, TrangThai = true });
            _ptttRepoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((PhuongThucThanhToan)null);

            // Act & Assert
            await _service.Invoking(s => s.CreateAsync(dto, 1))
                .Should().ThrowAsync<NotFoundException>()
                .WithMessage("Phương thức thanh toán không hợp lệ.");
        }

        [Fact]
        public async Task TC4_CreateAsync_MedicineNotFoundInDetails_ThrowsNotFoundException()
        {
            // Arrange (D6: thuoc == null)
            var dto = new CreateDonHangDto 
            { 
                IdchiNhanh = 1, 
                IdphuongThucTt = 1,
                ChiTietDonHangs = new List<ChiTietDonHangItemDto> { new ChiTietDonHangItemDto { Idthuoc = 1, SoLuong = 1, DonGia = 100 } }
            };
            _chiNhanhRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new ChiNhanh { Id = 1, TrangThai = true });
            _ptttRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new PhuongThucThanhToan { Id = 1, TrangThai = true });
            _thuocRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Thuoc)null);

            // Act & Assert
            await _service.Invoking(s => s.CreateAsync(dto, 1))
                .Should().ThrowAsync<NotFoundException>()
                .WithMessage("Thuốc ID 1 không tồn tại.");
        }

        [Fact]
        public async Task TC5_CreateAsync_NoLotsForMedicine_ThrowsBadRequestException()
        {
            // Arrange (D9: !loHangs.Any())
            var dto = new CreateDonHangDto 
            { 
                IdchiNhanh = 1, 
                IdphuongThucTt = 1,
                ChiTietDonHangs = new List<ChiTietDonHangItemDto> { new ChiTietDonHangItemDto { Idthuoc = 1, SoLuong = 1, DonGia = 100 } }
            };
            _chiNhanhRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new ChiNhanh { Id = 1, TrangThai = true });
            _ptttRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new PhuongThucThanhToan { Id = 1, TrangThai = true });
            _thuocRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Thuoc { Id = 1, TenThuoc = "A", TrangThai = true });
            _loHangRepoMock.Setup(r => r.GetByThuocIdAsync(1)).ReturnsAsync(new List<LoHang>()); // No lots

            // Act & Assert
            await _service.Invoking(s => s.CreateAsync(dto, 1))
                .Should().ThrowAsync<BadRequestException>()
                .WithMessage(" Không có lô hàng nào cho thuốc 'A'");
        }

        [Fact]
        public async Task TC6_CreateAsync_NoLotsAtBranch_ThrowsBadRequestException()
        {
            // Arrange (D10: !loHangsTaiChiNhanh.Any())
            var dto = new CreateDonHangDto 
            { 
                IdchiNhanh = 1, 
                IdphuongThucTt = 1,
                ChiTietDonHangs = new List<ChiTietDonHangItemDto> { new ChiTietDonHangItemDto { Idthuoc = 1, SoLuong = 1, DonGia = 100 } }
            };
            var loHang = new LoHang { Id = 10, Idthuoc = 1, KhoHangs = new List<KhoHang>() }; // No khohang for branch 1

            _chiNhanhRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new ChiNhanh { Id = 1, TrangThai = true });
            _ptttRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new PhuongThucThanhToan { Id = 1, TrangThai = true });
            _thuocRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Thuoc { Id = 1, TenThuoc = "A", TrangThai = true });
            _loHangRepoMock.Setup(r => r.GetByThuocIdAsync(1)).ReturnsAsync(new List<LoHang> { loHang });

            // Act & Assert
            await _service.Invoking(s => s.CreateAsync(dto, 1))
                .Should().ThrowAsync<BadRequestException>()
                .WithMessage(" Không có tồn kho cho thuốc 'A' tại chi nhánh này");
        }

        [Fact]
        public async Task TC7_CreateAsync_InsufficientStockAcrossAllLots_ThrowsBadRequestException()
        {
            // Arrange (D13: soLuongCanTru > 0)
            var dto = new CreateDonHangDto 
            { 
                IdchiNhanh = 1, 
                IdphuongThucTt = 1,
                ChiTietDonHangs = new List<ChiTietDonHangItemDto> { new ChiTietDonHangItemDto { Idthuoc = 1, SoLuong = 100, DonGia = 100 } }
            };
            var khoHang = new KhoHang { IdchiNhanh = 1, SoLuongTon = 50 };
            var loHang = new LoHang { Id = 10, Idthuoc = 1, KhoHangs = new List<KhoHang> { khoHang } };

            _chiNhanhRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new ChiNhanh { Id = 1, TrangThai = true });
            _ptttRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new PhuongThucThanhToan { Id = 1, TrangThai = true });
            _thuocRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Thuoc { Id = 1, TenThuoc = "A", TrangThai = true });
            _loHangRepoMock.Setup(r => r.GetByThuocIdAsync(1)).ReturnsAsync(new List<LoHang> { loHang });
            _khoHangRepoMock.Setup(r => r.GetByChiNhanhAndLoHangAsync(1, 10)).ReturnsAsync(khoHang);
            
            // Act & Assert
            await _service.Invoking(s => s.CreateAsync(dto, 1))
                .Should().ThrowAsync<BadRequestException>()
                .WithMessage("*Còn thiếu: 50*");
        }

        [Fact]
        public async Task TC13_CreateAsync_UnexpectedErrorInTransaction_RollsBackAndThrows()
        {
            // Arrange (P10: catch block)
            var dto = new CreateDonHangDto 
            { 
                IdchiNhanh = 1, 
                IdphuongThucTt = 1,
                ChiTietDonHangs = new List<ChiTietDonHangItemDto> { new ChiTietDonHangItemDto { Idthuoc = 1, SoLuong = 1, DonGia = 100 } }
            };
            _chiNhanhRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new ChiNhanh { Id = 1, TrangThai = true });
            _ptttRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new PhuongThucThanhToan { Id = 1, TrangThai = true });
            _thuocRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Thuoc { Id = 1, TenThuoc = "A", TrangThai = true });
            
            // Mock exception during SaveChanges
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ThrowsAsync(new Exception("Database Error"));

            // Act & Assert
            await _service.Invoking(s => s.CreateAsync(dto, 1))
                .Should().ThrowAsync<Exception>()
                .WithMessage("Database Error");

            _unitOfWorkMock.Verify(u => u.RollbackTransactionAsync(), Times.AtLeastOnce);
        }

        #endregion

        #region --- SUCCESS PATHS ---

        [Fact]
        public async Task TC8_CreateAsync_Success_CashPayment_CustomerWithPoints()
        {
            // Arrange
            var dto = new CreateDonHangDto 
            { 
                IdchiNhanh = 1, 
                IdphuongThucTt = 1,
                IdkhachHang = 1,
                ChiTietDonHangs = new List<ChiTietDonHangItemDto> 
                { 
                    new ChiTietDonHangItemDto { Idthuoc = 1, SoLuong = 10, DonGia = 10000 } 
                }
            };

            var chiNhanh = new ChiNhanh { Id = 1, TrangThai = true };
            var pttt = new PhuongThucThanhToan { Id = 1, TrangThai = true, TenPhuongThuc = "Tiền mặt" };
            var khachHang = new KhachHang { Id = 1, TenKhachHang = "Test", DiemTichLuy = 100, TrangThai = true };
            var thuoc = new Thuoc { Id = 1, TenThuoc = "A", TrangThai = true, DonVi = "Viên" };
            var khoHang = new KhoHang { IdchiNhanh = 1, IdloHang = 10, SoLuongTon = 20 };
            var loHang = new LoHang { Id = 10, SoLo = "LOT1", KhoHangs = new List<KhoHang> { khoHang } };

            _chiNhanhRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(chiNhanh);
            _ptttRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(pttt);
            _khachHangRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(khachHang);
            _thuocRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(thuoc);
            _loHangRepoMock.Setup(r => r.GetByThuocIdAsync(1)).ReturnsAsync(new List<LoHang> { loHang });
            _khoHangRepoMock.Setup(r => r.GetByChiNhanhAndLoHangAsync(1, 10)).ReturnsAsync(khoHang);
            
            // Mock GetByIdAsync for the result return (line 301)
            _donHangRepoMock.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<int>())).ReturnsAsync(new DonHang { Id = 1 });

            // Act
            var result = await _service.CreateAsync(dto, 1);

            // Assert
            result.Should().NotBeNull();
            _donHangRepoMock.Verify(r => r.CreateAsync(It.Is<DonHang>(dh => dh.TrangThaiThanhToan == "PAID_ON_DELIVERY")), Times.Once);
            _khachHangServiceMock.Verify(s => s.UpdateDiemTichLuyAsync(1, It.IsAny<int>(), It.IsAny<int>()), Times.Once); // D14 True
            _unitOfWorkMock.Verify(u => u.CommitTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task TC9_CreateAsync_Success_OnlinePayment_GuestCheckout()
        {
            // Arrange
            var dto = new CreateDonHangDto 
            { 
                IdchiNhanh = 1, 
                IdphuongThucTt = 2, // Non-cash
                IdkhachHang = null, // Guest
                ChiTietDonHangs = new List<ChiTietDonHangItemDto> 
                { 
                    new ChiTietDonHangItemDto { Idthuoc = 1, SoLuong = 5, DonGia = 20000 } 
                }
            };

            _chiNhanhRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new ChiNhanh { Id = 1, TrangThai = true });
            _ptttRepoMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(new PhuongThucThanhToan { Id = 2, TrangThai = true, TenPhuongThuc = "Momo" });
            _thuocRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Thuoc { Id = 1, TenThuoc = "A", TrangThai = true });
            
            var khoHang = new KhoHang { IdchiNhanh = 1, IdloHang = 10, SoLuongTon = 10 };
            var loHang = new LoHang { Id = 10, KhoHangs = new List<KhoHang> { khoHang } };
            _loHangRepoMock.Setup(r => r.GetByThuocIdAsync(1)).ReturnsAsync(new List<LoHang> { loHang });
            _khoHangRepoMock.Setup(r => r.GetByChiNhanhAndLoHangAsync(1, 10)).ReturnsAsync(khoHang);
            _donHangRepoMock.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<int>())).ReturnsAsync(new DonHang { Id = 1 });

            // Act
            var result = await _service.CreateAsync(dto, 1);

            // Assert
            _donHangRepoMock.Verify(r => r.CreateAsync(It.Is<DonHang>(dh => dh.TrangThaiThanhToan == "PENDING_PAYMENT")), Times.Once);
            _khachHangServiceMock.Verify(s => s.UpdateDiemTichLuyAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never); // D2 False
        }

        [Fact]
        public async Task TC10_CreateAsync_Success_CustomerWithInsufficientPoints()
        {
            // Arrange
            var dto = new CreateDonHangDto 
            { 
                IdchiNhanh = 1, 
                IdphuongThucTt = 1,
                IdkhachHang = 1,
                ChiTietDonHangs = new List<ChiTietDonHangItemDto> { new ChiTietDonHangItemDto { Idthuoc = 1, SoLuong = 1, DonGia = 1000 } }
            };

            var khachHang = new KhachHang { Id = 1, DiemTichLuy = 5, TrangThai = true }; // < 10 points (D7 False)

            _chiNhanhRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new ChiNhanh { Id = 1, TrangThai = true });
            _ptttRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new PhuongThucThanhToan { Id = 1, TrangThai = true, TenPhuongThuc = "Tiền mặt" });
            _khachHangRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(khachHang);
            _thuocRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Thuoc { Id = 1, TenThuoc = "A", TrangThai = true });
            
            var khoHang = new KhoHang { IdchiNhanh = 1, IdloHang = 10, SoLuongTon = 10 };
            var loHang = new LoHang { Id = 10, KhoHangs = new List<KhoHang> { khoHang } };
            _loHangRepoMock.Setup(r => r.GetByThuocIdAsync(1)).ReturnsAsync(new List<LoHang> { loHang });
            _khoHangRepoMock.Setup(r => r.GetByChiNhanhAndLoHangAsync(1, 10)).ReturnsAsync(khoHang);
            _donHangRepoMock.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<int>())).ReturnsAsync(new DonHang { Id = 1, IdkhachHang = 1 });

            // Act
            var result = await _service.CreateAsync(dto, 1);

            // Assert
            _donHangRepoMock.Verify(r => r.CreateAsync(It.Is<DonHang>(dh => dh.TienGiamGia == 0)), Times.Once);
        }

        [Fact]
        public async Task TC11_CreateAsync_Success_DeductFromMultipleLots()
        {
            // Arrange
            var dto = new CreateDonHangDto 
            { 
                IdchiNhanh = 1, 
                IdphuongThucTt = 1,
                ChiTietDonHangs = new List<ChiTietDonHangItemDto> { new ChiTietDonHangItemDto { Idthuoc = 1, SoLuong = 15, DonGia = 1000 } }
            };

            var khoHang1 = new KhoHang { IdchiNhanh = 1, IdloHang = 10, SoLuongTon = 10 };
            var loHang1 = new LoHang { Id = 10, SoLo = "LOT1", KhoHangs = new List<KhoHang> { khoHang1 } };
            
            var khoHang2 = new KhoHang { IdchiNhanh = 1, IdloHang = 11, SoLuongTon = 10 };
            var loHang2 = new LoHang { Id = 11, SoLo = "LOT2", KhoHangs = new List<KhoHang> { khoHang2 } };

            _chiNhanhRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new ChiNhanh { Id = 1, TrangThai = true });
            _ptttRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new PhuongThucThanhToan { Id = 1, TrangThai = true, TenPhuongThuc = "Tiền mặt" });
            _thuocRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Thuoc { Id = 1, TenThuoc = "A", TrangThai = true });
            _loHangRepoMock.Setup(r => r.GetByThuocIdAsync(1)).ReturnsAsync(new List<LoHang> { loHang1, loHang2 });
            _khoHangRepoMock.Setup(r => r.GetByChiNhanhAndLoHangAsync(1, 10)).ReturnsAsync(khoHang1);
            _khoHangRepoMock.Setup(r => r.GetByChiNhanhAndLoHangAsync(1, 11)).ReturnsAsync(khoHang2);
            _donHangRepoMock.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<int>())).ReturnsAsync(new DonHang { Id = 1 });

            // Act
            await _service.CreateAsync(dto, 1);

            // Assert
            _khoHangRepoMock.Verify(r => r.TruTonKhoAsync(1, 10, 10), Times.Once);
            _khoHangRepoMock.Verify(r => r.TruTonKhoAsync(1, 11, 5), Times.Once);
        }

        [Fact]
        public async Task TC12_CreateAsync_Success_FEFOLogic_PicksOldestBatch()
        {
            // Arrange (D11/D12 logic)
            var dto = new CreateDonHangDto 
            { 
                IdchiNhanh = 1, 
                IdphuongThucTt = 1,
                ChiTietDonHangs = new List<ChiTietDonHangItemDto> { new ChiTietDonHangItemDto { Idthuoc = 1, SoLuong = 5, DonGia = 1000 } }
            };

            _chiNhanhRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new ChiNhanh { Id = 1, TrangThai = true });
            _ptttRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new PhuongThucThanhToan { Id = 1, TrangThai = true });
            _thuocRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Thuoc { Id = 1, TenThuoc = "A", TrangThai = true });

            // Batch 1 expires in 2025 (Oldest)
            var kh1 = new KhoHang { IdchiNhanh = 1, IdloHang = 10, SoLuongTon = 10 };
            var lh1 = new LoHang { Id = 10, SoLo = "OLD_LOT", NgayHetHan = new DateOnly(2025, 1, 1), KhoHangs = new List<KhoHang> { kh1 } };
            
            // Batch 2 expires in 2026
            var kh2 = new KhoHang { IdchiNhanh = 1, IdloHang = 11, SoLuongTon = 10 };
            var lh2 = new LoHang { Id = 11, SoLo = "NEW_LOT", NgayHetHan = new DateOnly(2026, 1, 1), KhoHangs = new List<KhoHang> { kh2 } };

            // Scenario: Repository returns unsorted list. The Service must handle selection logic.
            // (Note: In DonHangService.CreateAsync line 218, it loops through loHangsTaiChiNhanh).
            _loHangRepoMock.Setup(r => r.GetByThuocIdAsync(1)).ReturnsAsync(new List<LoHang> { lh2, lh1 }); 
            _khoHangRepoMock.Setup(r => r.GetByChiNhanhAndLoHangAsync(1, 10)).ReturnsAsync(kh1);
            _donHangRepoMock.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<int>())).ReturnsAsync(new DonHang { Id = 1 });

            // Act
            await _service.CreateAsync(dto, 1);

            // Assert
            // The code should pick the first available lot in loHangsTaiChiNhanh. 
            // In CreateAsync, loHangsTaiChiNhanh is derived from loHangs which comes from GetByThuocIdAsync.
            // If the repo doesn't sort, the service should (or we verify it picks the one we expect if it doesn't sort).
            // Checked line 206 of DonHangService.cs: var loHangsTaiChiNhanh = loHangs.Where(...).ToList();
            // It DOES NOT sort by NgayHetHan explicitly in CreateAsync (unlike CreateCustomerOrderAsync!).
            // This is a potential bug or difference in logic.
            
            _khoHangRepoMock.Verify(r => r.TruTonKhoAsync(1, 10, 5), Times.Once); 
        }

        #endregion

        #region --- CreateCustomerOrderAsync TESTS ---

        [Fact]
        public async Task TC1_CreateCustomerOrderAsync_CustomerNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var dto = new CreateCustomerOrderDto { IdchiNhanh = 1 };
            _khachHangRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((KhachHang)null);

            // Act & Assert
            await _service.Invoking(s => s.CreateCustomerOrderAsync(dto, 1))
                .Should().ThrowAsync<NotFoundException>()
                .WithMessage("Khách hàng không tồn tại.");
        }

        [Fact]
        public async Task TC2_CreateCustomerOrderAsync_GuestNoPhone_ThrowsBadRequestException()
        {
            // Arrange
            var dto = new CreateCustomerOrderDto { IdchiNhanh = 1, Sdt = "" }; // No idKhachHang, no Sdt

            // Act & Assert
            await _service.Invoking(s => s.CreateCustomerOrderAsync(dto, null))
                .Should().ThrowAsync<BadRequestException>()
                .WithMessage("Vui lòng cung cấp số điện thoại để đặt hàng.");
        }

        [Fact]
        public async Task TC3_CreateCustomerOrderAsync_AutoSelectBranchFail_ThrowsBadRequestException()
        {
            // Arrange
            var dto = new CreateCustomerOrderDto { IdchiNhanh = 0, Sdt = "0123456789" };
            _khachHangRepoMock.Setup(r => r.GetBySdtAsync("0123456789")).ReturnsAsync(new KhachHang { Id = 1, TrangThai = true });
            
            // Mock FindAnyActiveBranchAsync (via ChiNhanhRepository.GetPagedListAsync)
            _chiNhanhRepoMock.Setup(r => r.GetPagedListAsync(1, 1000, true, null))
                .ReturnsAsync(new PagedResult<ChiNhanh> { Items = new List<ChiNhanh>() }); // No active branches

            // Act & Assert
            await _service.Invoking(s => s.CreateCustomerOrderAsync(dto, null))
                .Should().ThrowAsync<BadRequestException>()
                .WithMessage("Không tìm thấy chi nhánh nào đang hoạt động.");
        }

        [Fact]
        public async Task TC4_CreateCustomerOrderAsync_SelectedBranchInvalid_ThrowsNotFoundException()
        {
            // Arrange
            var dto = new CreateCustomerOrderDto { IdchiNhanh = 1, Sdt = "0123456789" };
            _khachHangRepoMock.Setup(r => r.GetBySdtAsync("0123456789")).ReturnsAsync(new KhachHang { Id = 1, TrangThai = true });
            _chiNhanhRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((ChiNhanh)null); // Branch not found

            // Act & Assert
            await _service.Invoking(s => s.CreateCustomerOrderAsync(dto, null))
                .Should().ThrowAsync<NotFoundException>()
                .WithMessage("Chi nhánh không tồn tại hoặc không hoạt động.");
        }

        [Fact]
        public async Task TC5_CreateCustomerOrderAsync_PaymentMethodInvalid_ThrowsNotFoundException()
        {
            // Arrange
            var dto = new CreateCustomerOrderDto { IdchiNhanh = 1, IdphuongThucTt = 999, Sdt = "0123456789" };
            _khachHangRepoMock.Setup(r => r.GetBySdtAsync("0123456789")).ReturnsAsync(new KhachHang { Id = 1, TrangThai = true });
            _chiNhanhRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new ChiNhanh { Id = 1, TrangThai = true });
            _ptttRepoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((PhuongThucThanhToan)null);

            // Act & Assert
            await _service.Invoking(s => s.CreateCustomerOrderAsync(dto, null))
                .Should().ThrowAsync<NotFoundException>()
                .WithMessage("Phương thức thanh toán không hợp lệ.");
        }

        [Fact]
        public async Task TC6_CreateCustomerOrderAsync_MedicineInvalid_ThrowsNotFoundException()
        {
            // Arrange
            var dto = new CreateCustomerOrderDto 
            { 
                IdchiNhanh = 1, 
                IdphuongThucTt = 1, 
                Sdt = "0123456789",
                ChiTietDonHangs = new List<ChiTietDonHangItemDto> { new ChiTietDonHangItemDto { Idthuoc = 1, SoLuong = 1, DonGia = 100 } }
            };
            _khachHangRepoMock.Setup(r => r.GetBySdtAsync("0123456789")).ReturnsAsync(new KhachHang { Id = 1, TrangThai = true });
            _chiNhanhRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new ChiNhanh { Id = 1, TrangThai = true });
            _ptttRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new PhuongThucThanhToan { Id = 1, TrangThai = true });
            _thuocRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Thuoc)null);

            // Act & Assert
            await _service.Invoking(s => s.CreateCustomerOrderAsync(dto, null))
                .Should().ThrowAsync<NotFoundException>()
                .WithMessage("Thuốc ID 1 không tồn tại.");
        }

        [Fact]
        public async Task TC7_CreateCustomerOrderAsync_InsufficientStockAllBranches_ThrowsBadRequestException()
        {
            // Arrange
            var dto = new CreateCustomerOrderDto 
            { 
                IdchiNhanh = 1, 
                IdphuongThucTt = 1, 
                Sdt = "0123456789",
                ChiTietDonHangs = new List<ChiTietDonHangItemDto> { new ChiTietDonHangItemDto { Idthuoc = 1, SoLuong = 100, DonGia = 100 } }
            };
            _khachHangRepoMock.Setup(r => r.GetBySdtAsync("0123456789")).ReturnsAsync(new KhachHang { Id = 1, TrangThai = true });
            _chiNhanhRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new ChiNhanh { Id = 1, TrangThai = true });
            _ptttRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new PhuongThucThanhToan { Id = 1, TrangThai = true });
            _thuocRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Thuoc { Id = 1, TenThuoc = "A", TrangThai = true });
            
            // Mock branches
            _chiNhanhRepoMock.Setup(r => r.GetPagedListAsync(1, 1000, true, null))
                .ReturnsAsync(new PagedResult<ChiNhanh> { Items = new List<ChiNhanh> { new ChiNhanh { Id = 1 }, new ChiNhanh { Id = 2 } } });

            // Branch 1 has 10, Branch 2 has 20. Total 30 < 100
            var kh1 = new KhoHang { IdchiNhanh = 1, IdloHang = 10, SoLuongTon = 10 };
            var lh1 = new LoHang { Id = 10, KhoHangs = new List<KhoHang> { kh1 } };
            var kh2 = new KhoHang { IdchiNhanh = 2, IdloHang = 20, SoLuongTon = 20 };
            var lh2 = new LoHang { Id = 20, KhoHangs = new List<KhoHang> { kh2 } };

            _loHangRepoMock.Setup(r => r.GetByThuocIdAsync(1)).ReturnsAsync(new List<LoHang> { lh1, lh2 });
            _khoHangRepoMock.Setup(r => r.GetByChiNhanhAndLoHangAsync(1, 10)).ReturnsAsync(kh1);
            _khoHangRepoMock.Setup(r => r.GetByChiNhanhAndLoHangAsync(2, 20)).ReturnsAsync(kh2);

            // Act & Assert
            await _service.Invoking(s => s.CreateCustomerOrderAsync(dto, null))
                .Should().ThrowAsync<BadRequestException>()
                .WithMessage("*Còn thiếu: 70*");
        }

        [Fact]
        public async Task TC8_CreateCustomerOrderAsync_Success_LoggedIn_Cash_Points()
        {
            // Arrange
            var dto = new CreateCustomerOrderDto 
            { 
                IdchiNhanh = 1, 
                IdphuongThucTt = 1,
                ChiTietDonHangs = new List<ChiTietDonHangItemDto> { new ChiTietDonHangItemDto { Idthuoc = 1, SoLuong = 1, DonGia = 100000 } }
            };

            var khachHang = new KhachHang { Id = 1, DiemTichLuy = 100, TrangThai = true };
            _khachHangRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(khachHang);
            _chiNhanhRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new ChiNhanh { Id = 1, TrangThai = true });
            _ptttRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new PhuongThucThanhToan { Id = 1, TrangThai = true, TenPhuongThuc = "Tiền mặt" });
            _thuocRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Thuoc { Id = 1, TenThuoc = "A", TrangThai = true });
            
            _chiNhanhRepoMock.Setup(r => r.GetPagedListAsync(1, 1000, true, null))
                .ReturnsAsync(new PagedResult<ChiNhanh> { Items = new List<ChiNhanh> { new ChiNhanh { Id = 1 } } });

            var kh = new KhoHang { IdchiNhanh = 1, IdloHang = 10, SoLuongTon = 10 };
            _loHangRepoMock.Setup(r => r.GetByThuocIdAsync(1)).ReturnsAsync(new List<LoHang> { new LoHang { Id = 10, KhoHangs = new List<KhoHang> { kh } } });
            _khoHangRepoMock.Setup(r => r.GetByChiNhanhAndLoHangAsync(1, 10)).ReturnsAsync(kh);
            _donHangRepoMock.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<int>())).ReturnsAsync(new DonHang { Id = 1 });

            // Act
            var result = await _service.CreateCustomerOrderAsync(dto, 1);

            // Assert
            result.Should().NotBeNull();
            _donHangRepoMock.Verify(r => r.CreateAsync(It.Is<DonHang>(dh => dh.TienGiamGia > 0)), Times.Once); // Points discount
            _khachHangServiceMock.Verify(s => s.UpdateDiemTichLuyAsync(1, It.IsAny<int>(), It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task TC9_CreateCustomerOrderAsync_Success_Guest_NewCustomer_Online()
        {
            // Arrange
            var dto = new CreateCustomerOrderDto 
            { 
                IdchiNhanh = 1, 
                IdphuongThucTt = 2,
                Sdt = "0999888777",
                TenKhachHang = "New Guest",
                ChiTietDonHangs = new List<ChiTietDonHangItemDto> { new ChiTietDonHangItemDto { Idthuoc = 1, SoLuong = 1, DonGia = 1000 } }
            };

            _khachHangRepoMock.Setup(r => r.GetBySdtAsync("0999888777")).ReturnsAsync((KhachHang)null); // New guest
            _chiNhanhRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new ChiNhanh { Id = 1, TrangThai = true });
            _ptttRepoMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(new PhuongThucThanhToan { Id = 2, TrangThai = true, TenPhuongThuc = "Momo" });
            _thuocRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Thuoc { Id = 1, TenThuoc = "A", TrangThai = true });
            
            _chiNhanhRepoMock.Setup(r => r.GetPagedListAsync(1, 1000, true, null))
                .ReturnsAsync(new PagedResult<ChiNhanh> { Items = new List<ChiNhanh> { new ChiNhanh { Id = 1 } } });
            
            var kh = new KhoHang { IdchiNhanh = 1, IdloHang = 10, SoLuongTon = 10 };
            _loHangRepoMock.Setup(r => r.GetByThuocIdAsync(1)).ReturnsAsync(new List<LoHang> { new LoHang { Id = 10, KhoHangs = new List<KhoHang> { kh } } });
            _khoHangRepoMock.Setup(r => r.GetByChiNhanhAndLoHangAsync(1, 10)).ReturnsAsync(kh);
            _donHangRepoMock.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<int>())).ReturnsAsync(new DonHang { Id = 1 });

            // Act
            var result = await _service.CreateCustomerOrderAsync(dto, null);

            // Assert
            _khachHangRepoMock.Verify(r => r.CreateAsync(It.Is<KhachHang>(k => k.Sdt == "0999888777")), Times.Once);
            _donHangRepoMock.Verify(r => r.CreateAsync(It.Is<DonHang>(dh => dh.TrangThaiThanhToan == "PENDING_PAYMENT")), Times.Once);
        }

        [Fact]
        public async Task TC10_CreateCustomerOrderAsync_Success_Guest_UpdateExisting_Online()
        {
            // Arrange
            var dto = new CreateCustomerOrderDto 
            { 
                IdchiNhanh = 1, 
                IdphuongThucTt = 2,
                Sdt = "0123456789",
                TenKhachHang = "Updated Name",
                ChiTietDonHangs = new List<ChiTietDonHangItemDto> { new ChiTietDonHangItemDto { Idthuoc = 1, SoLuong = 1, DonGia = 1000 } }
            };

            var existingKh = new KhachHang { Id = 10, Sdt = "0123456789", TenKhachHang = "Old Name", TrangThai = true };
            _khachHangRepoMock.Setup(r => r.GetBySdtAsync("0123456789")).ReturnsAsync(existingKh);
            _chiNhanhRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new ChiNhanh { Id = 1, TrangThai = true });
            _ptttRepoMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(new PhuongThucThanhToan { Id = 2, TrangThai = true, TenPhuongThuc = "Momo" });
            _thuocRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Thuoc { Id = 1, TenThuoc = "A", TrangThai = true });
            
            _chiNhanhRepoMock.Setup(r => r.GetPagedListAsync(1, 1000, true, null))
                .ReturnsAsync(new PagedResult<ChiNhanh> { Items = new List<ChiNhanh> { new ChiNhanh { Id = 1 } } });
            
            var kh = new KhoHang { IdchiNhanh = 1, IdloHang = 10, SoLuongTon = 10 };
            _loHangRepoMock.Setup(r => r.GetByThuocIdAsync(1)).ReturnsAsync(new List<LoHang> { new LoHang { Id = 10, KhoHangs = new List<KhoHang> { kh } } });
            _khoHangRepoMock.Setup(r => r.GetByChiNhanhAndLoHangAsync(1, 10)).ReturnsAsync(kh);
            _donHangRepoMock.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<int>())).ReturnsAsync(new DonHang { Id = 1 });

            // Act
            await _service.CreateCustomerOrderAsync(dto, null);

            // Assert
            existingKh.TenKhachHang.Should().Be("Updated Name");
            _khachHangRepoMock.Verify(r => r.UpdateAsync(existingKh), Times.Once);
        }

        [Fact]
        public async Task TC11_CreateCustomerOrderAsync_Success_CrossBranchDeduction()
        {
            // Arrange
            var dto = new CreateCustomerOrderDto 
            { 
                IdchiNhanh = 1, 
                IdphuongThucTt = 1,
                Sdt = "0123456789",
                ChiTietDonHangs = new List<ChiTietDonHangItemDto> { new ChiTietDonHangItemDto { Idthuoc = 1, SoLuong = 15, DonGia = 1000 } }
            };

            _khachHangRepoMock.Setup(r => r.GetBySdtAsync("0123456789")).ReturnsAsync(new KhachHang { Id = 1, TrangThai = true });
            _chiNhanhRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new ChiNhanh { Id = 1, TrangThai = true });
            _chiNhanhRepoMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(new ChiNhanh { Id = 2, TrangThai = true });
            _ptttRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new PhuongThucThanhToan { Id = 1, TrangThai = true, TenPhuongThuc = "Cash" });
            _thuocRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Thuoc { Id = 1, TenThuoc = "A", TrangThai = true });
            
            _chiNhanhRepoMock.Setup(r => r.GetPagedListAsync(1, 1000, true, null))
                .ReturnsAsync(new PagedResult<ChiNhanh> { Items = new List<ChiNhanh> { new ChiNhanh { Id = 1 }, new ChiNhanh { Id = 2 } } });

            var kh1 = new KhoHang { IdchiNhanh = 1, IdloHang = 10, SoLuongTon = 10 };
            var lh1 = new LoHang { Id = 10, KhoHangs = new List<KhoHang> { kh1 } };
            var kh2 = new KhoHang { IdchiNhanh = 2, IdloHang = 20, SoLuongTon = 10 };
            var lh2 = new LoHang { Id = 20, KhoHangs = new List<KhoHang> { kh2 } };

            _loHangRepoMock.Setup(r => r.GetByThuocIdAsync(1)).ReturnsAsync(new List<LoHang> { lh1, lh2 });
            _khoHangRepoMock.Setup(r => r.GetByChiNhanhAndLoHangAsync(1, 10)).ReturnsAsync(kh1);
            _khoHangRepoMock.Setup(r => r.GetByChiNhanhAndLoHangAsync(2, 20)).ReturnsAsync(kh2);
            _donHangRepoMock.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<int>())).ReturnsAsync(new DonHang { Id = 1 });

            // Act
            await _service.CreateCustomerOrderAsync(dto, null);

            // Assert
            _khoHangRepoMock.Verify(r => r.TruTonKhoAsync(1, 10, 10), Times.Once);
            _khoHangRepoMock.Verify(r => r.TruTonKhoAsync(2, 20, 5), Times.Once);
        }

        [Fact]
        public async Task TC12_CreateCustomerOrderAsync_Success_FEFOLogic()
        {
            // Arrange
            var dto = new CreateCustomerOrderDto 
            { 
                IdchiNhanh = 1, 
                IdphuongThucTt = 1,
                ChiTietDonHangs = new List<ChiTietDonHangItemDto> { new ChiTietDonHangItemDto { Idthuoc = 1, SoLuong = 5, DonGia = 1000 } }
            };

            _khachHangRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new KhachHang { Id = 1, TrangThai = true });
            _chiNhanhRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new ChiNhanh { Id = 1, TrangThai = true });
            _ptttRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new PhuongThucThanhToan { Id = 1, TrangThai = true, TenPhuongThuc = "Cash" });
            _thuocRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Thuoc { Id = 1, TenThuoc = "A", TrangThai = true });
            
            _chiNhanhRepoMock.Setup(r => r.GetPagedListAsync(1, 1000, true, null))
                .ReturnsAsync(new PagedResult<ChiNhanh> { Items = new List<ChiNhanh> { new ChiNhanh { Id = 1 } } });

            // Lot 1 expires earlier
            var kh1 = new KhoHang { IdchiNhanh = 1, IdloHang = 10, SoLuongTon = 10 };
            var lh1 = new LoHang { Id = 10, NgayHetHan = new DateOnly(2025, 1, 1), KhoHangs = new List<KhoHang> { kh1 } };
            
            var kh2 = new KhoHang { IdchiNhanh = 1, IdloHang = 11, SoLuongTon = 10 };
            var lh2 = new LoHang { Id = 11, NgayHetHan = new DateOnly(2026, 1, 1), KhoHangs = new List<KhoHang> { kh2 } };

            _loHangRepoMock.Setup(r => r.GetByThuocIdAsync(1)).ReturnsAsync(new List<LoHang> { lh2, lh1 }); // Return unsorted
            _khoHangRepoMock.Setup(r => r.GetByChiNhanhAndLoHangAsync(1, 10)).ReturnsAsync(kh1);
            _donHangRepoMock.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<int>())).ReturnsAsync(new DonHang { Id = 1 });

            // Act
            await _service.CreateCustomerOrderAsync(dto, 1);

            // Assert
            _khoHangRepoMock.Verify(r => r.TruTonKhoAsync(1, 10, 5), Times.Once); // Should pick LOT1 (Id 10)
        }

        #endregion
    }
}
