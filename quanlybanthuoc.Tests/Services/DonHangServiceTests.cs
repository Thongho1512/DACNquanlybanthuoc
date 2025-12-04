using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using quanlybanthuoc.Data.Entities;
using quanlybanthuoc.Data.Repositories;
using quanlybanthuoc.Dtos.DonHang;
using quanlybanthuoc.Middleware.Exceptions;
using quanlybanthuoc.Services.Impl;
using Xunit;

namespace quanlybanthuoc.Tests.Services
{
    public class DonHangServiceTests
    {
        [Fact]
        public async Task CreateAsync_Success_WithStockAndCustomerPoints()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var chiNhanhRepo = new Mock<IChiNhanhRepository>();
            var khachHangRepo = new Mock<IKhachHangRepository>();
            var phuongThucRepo = new Mock<IPhuongThucThanhToanRepository>();
            var thuocRepo = new Mock<IThuocRepository>();
            var loHangRepo = new Mock<ILoHangRepository>();
            var khoHangRepo = new Mock<IKhoHangRepository>();
            var donHangRepo = new Mock<IDonHangRepository>();
            var chiTietRepo = new Mock<IChiTietDonHangRepository>();
            var lichSuDiemRepo = new Mock<ILichSuDiemRepository>();

            unitOfWorkMock.SetupGet(u => u.ChiNhanhRepository).Returns(chiNhanhRepo.Object);
            unitOfWorkMock.SetupGet(u => u.KhachHangRepository).Returns(khachHangRepo.Object);
            unitOfWorkMock.SetupGet(u => u.PhuongThucThanhToanRepository).Returns(phuongThucRepo.Object);
            unitOfWorkMock.SetupGet(u => u.ThuocRepository).Returns(thuocRepo.Object);
            unitOfWorkMock.SetupGet(u => u.LoHangRepository).Returns(loHangRepo.Object);
            unitOfWorkMock.SetupGet(u => u.KhoHangRepository).Returns(khoHangRepo.Object);
            unitOfWorkMock.SetupGet(u => u.DonHangRepository).Returns(donHangRepo.Object);
            unitOfWorkMock.SetupGet(u => u.ChiTietDonHangRepository).Returns(chiTietRepo.Object);
            unitOfWorkMock.SetupGet(u => u.LichSuDiemRepository).Returns(lichSuDiemRepo.Object);

            unitOfWorkMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            unitOfWorkMock.Setup(u => u.CommitTransactionAsync()).Returns(Task.CompletedTask);
            unitOfWorkMock.Setup(u => u.RollbackTransactionAsync()).Returns(Task.CompletedTask);
            unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            // Data
            var dto = new CreateDonHangDto
            {
                IdchiNhanh = 1,
                IdphuongThucTt = 1,
                IdkhachHang = 1,
                ChiTietDonHangs = new List<CreateDonHangDto.ChiTietDonHangItemDto>
                {
                    new CreateDonHangDto.ChiTietDonHangItemDto
                    {
                        Idthuoc = 1,
                        SoLuong = 2,
                        DonGia = 10000m
                    }
                }
            };

            chiNhanhRepo.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(new ChiNhanh { Id = 1, TrangThai = true });

            phuongThucRepo.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(new PhuongThucThanhToan { Id = 1, TrangThai = true });

            khachHangRepo.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(new KhachHang { Id = 1, DiemTichLuy = 20, TrangThai = true });

            thuocRepo.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(new Thuoc { Id = 1, TenThuoc = "Thuoc A", DonVi = "viên", TrangThai = true });

            var loHang = new LoHang
            {
                Id = 10,
                SoLo = "L01",
                NgayHetHan = DateOnly.FromDateTime(DateTime.Now.AddDays(30)),
                KhoHangs = new List<KhoHang>
                {
                    new KhoHang { Id = 100, IdchiNhanh = 1, SoLuongTon = 10 }
                }
            };

            loHangRepo.Setup(r => r.GetByThuocIdAsync(1)).ReturnsAsync(new List<LoHang> { loHang });

            khoHangRepo.Setup(r => r.GetByChiNhanhAndLoHangAsync(1, 10))
                .ReturnsAsync(new KhoHang { Id = 100, IdchiNhanh = 1, SoLuongTon = 10 });

            // When creating DonHang, set Id to 1 (simulate EF behavior)
            donHangRepo.Setup(r => r.CreateAsync(It.IsAny<DonHang>()))
                .ReturnsAsync((DonHang dh) =>
                {
                    dh.Id = 1;
                    return dh;
                });

            // Return the saved order with details when GetByIdWithDetailsAsync is called
            donHangRepo.Setup(r => r.GetByIdWithDetailsAsync(1))
                .ReturnsAsync(new DonHang
                {
                    Id = 1,
                    IdnguoiDung = 5,
                    IdkhachHang = 1,
                    IdchiNhanh = 1,
                    IdphuongThucTt = 1,
                    TongTien = 20000m,
                    TienGiamGia = 10000m,
                    ThanhTien = 10000m,
                    NgayTao = DateOnly.FromDateTime(DateTime.Now),
                    ChiTietDonHangs = new List<ChiTietDonHang>
                    {
                        new ChiTietDonHang
                        {
                            Id = 1,
                            Idthuoc = 1,
                            SoLuong = 2,
                            DonGia = 10000m,
                            ThanhTien = 20000m,
                            IdthuocNavigation = new Thuoc { TenThuoc = "Thuoc A", DonVi = "viên" }
                        }
                    },
                    IdkhachHangNavigation = new KhachHang { TenKhachHang = "KH 1" },
                    IdchiNhanhNavigation = new ChiNhanh { TenChiNhanh = "CN 1" },
                    IdphuongThucTtNavigation = new PhuongThucThanhToan { TenPhuongThuc = "Cash" }
                });

            var khachHangServiceMock = new Mock<quanlybanthuoc.Services.IKhachHangService>();
            khachHangServiceMock.Setup(k => k.UpdateDiemTichLuyAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(0);

            var loggerMock = new Mock<ILogger<DonHangService>>();
            var mapperMock = new Mock<IMapper>();

            var service = new DonHangService(
                unitOfWorkMock.Object,
                loggerMock.Object,
                mapperMock.Object,
                khachHangServiceMock.Object);

            // Act
            var result = await service.CreateAsync(dto, idNguoiDung: 5);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(20000m, result.TongTien);
            Assert.Equal(10000m, result.TienGiamGia);
            Assert.Equal(10000m, result.ThanhTien);
            Assert.Single(result.ChiTietDonHangs);
            // Verify stock deduction invoked for expected quantity
            khoHangRepo.Verify(k => k.TruTonKhoAsync(1, 10, 2), Times.Once);
            // Verify transaction commit
            unitOfWorkMock.Verify(u => u.CommitTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ThrowsNotFound_WhenChiNhanhMissing()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var chiNhanhRepo = new Mock<IChiNhanhRepository>();
            unitOfWorkMock.SetupGet(u => u.ChiNhanhRepository).Returns(chiNhanhRepo.Object);

            // chi nhánh không tồn tại
            chiNhanhRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((ChiNhanh?)null);

            var khachHangServiceMock = new Mock<quanlybanthuoc.Services.IKhachHangService>();
            var loggerMock = new Mock<ILogger<DonHangService>>();
            var mapperMock = new Mock<IMapper>();

            var service = new DonHangService(
                unitOfWorkMock.Object,
                loggerMock.Object,
                mapperMock.Object,
                khachHangServiceMock.Object);

            var dto = new CreateDonHangDto { IdchiNhanh = 999, IdphuongThucTt = 1 };

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(async () =>
                await service.CreateAsync(dto, idNguoiDung: 1));
        }
    }
}