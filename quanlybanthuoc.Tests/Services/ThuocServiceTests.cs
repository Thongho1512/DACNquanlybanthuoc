using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using quanlybanthuoc.Data.Entities;
using quanlybanthuoc.Data.Repositories;
using quanlybanthuoc.Dtos.Thuoc;
using quanlybanthuoc.Middleware.Exceptions;
using quanlybanthuoc.Services.Impl;

namespace quanlybanthuoc.Tests.Services
{
    /// <summary>
    /// Unit Tests cho ThuocService - Chức năng CreateAsync
    /// 
    /// Áp dụng 3 phương pháp:
    /// 1. Equivalence Partitioning (EP) - Phân vùng tương đương
    /// 2. Boundary Value Analysis (BVA) - Phân tích giá trị biên
    /// 3. Decision Table Testing - Bảng quyết định
    /// </summary>
    public class ThuocServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<ThuocService>> _loggerMock;
        private readonly Mock<IThuocRepository> _thuocRepoMock;
        private readonly Mock<IDanhMucRepository> _danhMucRepoMock;
        private readonly ThuocService _service;

        public ThuocServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<ThuocService>>();
            _thuocRepoMock = new Mock<IThuocRepository>();
            _danhMucRepoMock = new Mock<IDanhMucRepository>();

            _unitOfWorkMock.Setup(u => u.ThuocRepository).Returns(_thuocRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.DanhMucRepository).Returns(_danhMucRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            _service = new ThuocService(
                _unitOfWorkMock.Object,
                _loggerMock.Object,
                _mapperMock.Object
            );
        }

        #region EQUIVALENCE PARTITIONING - Giá Bán

        /// <summary>
        /// EP Test Case 1: Invalid Partition (Giá bán ≤ 0)
        /// 
        /// Partition: ≤ 0
        /// Đại diện: 0, -1000
        /// Kỳ vọng: BadRequestException
        /// </summary>
        [Theory]
        [InlineData(0)]        // EP: Giá = 0 (invalid)
        [InlineData(-1000)]    // EP: Giá âm (invalid)
        public async Task CreateAsync_EP_GiaBanInvalid_ThrowsBadRequestException(decimal giaBan)
        {
            // Arrange
            var createDto = new CreateThuocDto
            {
                TenThuoc = "Paracetamol 500mg",
                GiaBan = giaBan,
                DonVi = "Viên",
                IddanhMuc = 1
            };

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(
                async () => await _service.CreateAsync(createDto)
            );
        }

        /// <summary>
        /// EP Test Case 2: Valid Partition - Low Range (1 - 100,000)
        /// 
        /// Partition: Thuốc thông thường
        /// Đại diện: 1 (đầu), 50,000 (giữa), 100,000 (cuối)
        /// Kỳ vọng: Success
        /// </summary>
        [Theory]
        [InlineData(1)]        // EP: Giá rất thấp (đầu partition)
        [InlineData(50000)]    // EP: Giá trung bình (giữa partition)
        [InlineData(100000)]   // EP: Giá cao (cuối partition)
        public async Task CreateAsync_EP_GiaBanValidLow_Success(decimal giaBan)
        {
            // Arrange
            var createDto = new CreateThuocDto
            {
                TenThuoc = "Paracetamol 500mg",
                GiaBan = giaBan,
                DonVi = "Viên",
                IddanhMuc = 1
            };

            SetupSuccessfulCreate(createDto, giaBan);

            // Act
            var result = await _service.CreateAsync(createDto);

            // Assert
            result.Should().NotBeNull();
            result.GiaBan.Should().Be(giaBan);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        /// <summary>
        /// EP Test Case 3: Valid Partition - Mid Range (100,001 - 1,000,000)
        /// 
        /// Partition: Thuốc đặc biệt
        /// Đại diện: 100,001 (đầu), 500,000 (giữa), 1,000,000 (cuối)
        /// Kỳ vọng: Success
        /// </summary>
        [Theory]
        [InlineData(100001)]   // EP: Đầu partition mid
        [InlineData(500000)]   // EP: Giữa partition mid
        [InlineData(1000000)]  // EP: Cuối partition mid
        public async Task CreateAsync_EP_GiaBanValidMid_Success(decimal giaBan)
        {
            // Arrange
            var createDto = new CreateThuocDto
            {
                TenThuoc = "Insulin",
                GiaBan = giaBan,
                DonVi = "Lọ",
                IddanhMuc = 1
            };

            SetupSuccessfulCreate(createDto, giaBan);

            // Act
            var result = await _service.CreateAsync(createDto);

            // Assert
            result.Should().NotBeNull();
            result.GiaBan.Should().Be(giaBan);
        }

        /// <summary>
        /// EP Test Case 4: Valid Partition - High Range (1,000,001 - 10,000,000)
        /// 
        /// Partition: Thuốc cao cấp
        /// Đại diện: 1,000,001 (đầu), 5,000,000 (giữa), 10,000,000 (cuối)
        /// Kỳ vọng: Success
        /// </summary>
        [Theory]
        [InlineData(1000001)]   // EP: Đầu partition high
        [InlineData(5000000)]   // EP: Giữa partition high
        [InlineData(10000000)]  // EP: Cuối partition high
        public async Task CreateAsync_EP_GiaBanValidHigh_Success(decimal giaBan)
        {
            // Arrange
            var createDto = new CreateThuocDto
            {
                TenThuoc = "Thuốc điều trị ung thư",
                GiaBan = giaBan,
                DonVi = "Lọ",
                IddanhMuc = 1
            };

            SetupSuccessfulCreate(createDto, giaBan);

            // Act
            var result = await _service.CreateAsync(createDto);

            // Assert
            result.Should().NotBeNull();
            result.GiaBan.Should().Be(giaBan);
        }

        /// <summary>
        /// EP Test Case 5: Invalid Partition (Giá bán > 10,000,000)
        /// 
        /// Partition: > 10,000,000 (vượt giới hạn)
        /// Đại diện: 10,000,001, 15,000,000
        /// Kỳ vọng: BadRequestException
        /// </summary>
        [Theory]
        [InlineData(10000001)]  // EP: Vượt giới hạn 1đ
        [InlineData(15000000)]  // EP: Vượt giới hạn nhiều
        public async Task CreateAsync_EP_GiaBanTooHigh_ThrowsBadRequestException(decimal giaBan)
        {
            // Arrange
            var createDto = new CreateThuocDto
            {
                TenThuoc = "Test",
                GiaBan = giaBan,
                DonVi = "Viên",
                IddanhMuc = 1
            };

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(
                async () => await _service.CreateAsync(createDto)
            );
        }

        #endregion

        #region BOUNDARY VALUE ANALYSIS - Giá Bán (Low Range: 1 - 100,000)

        /// <summary>
        /// BVA Test Case 1: Lower Boundary của Valid Low Range
        /// 
        /// Biên dưới: 0, 1, 2
        /// - 0: Dưới biên (invalid)
        /// - 1: Tại biên hợp lệ (valid)
        /// - 2: Trên biên (valid)
        /// </summary>
        [Theory]
        [InlineData(0, false)]   // BVA: Dưới biên (invalid)
        [InlineData(1, true)]    // BVA: Tại biên hợp lệ (valid)
        [InlineData(2, true)]    // BVA: Trên biên (valid)
        public async Task CreateAsync_BVA_GiaBanLowerBoundary_ReturnsExpectedResult(
            decimal giaBan,
            bool shouldSucceed)
        {
            // Arrange
            var createDto = new CreateThuocDto
            {
                TenThuoc = "Paracetamol 500mg",
                GiaBan = giaBan,
                DonVi = "Viên",
                IddanhMuc = 1
            };

            if (shouldSucceed)
            {
                SetupSuccessfulCreate(createDto, giaBan);
            }

            // Act & Assert
            if (shouldSucceed)
            {
                var result = await _service.CreateAsync(createDto);
                result.Should().NotBeNull();
                result.GiaBan.Should().Be(giaBan);
            }
            else
            {
                await Assert.ThrowsAsync<BadRequestException>(
                    async () => await _service.CreateAsync(createDto)
                );
            }
        }

        /// <summary>
        /// BVA Test Case 2: Upper Boundary của Valid Low Range
        /// 
        /// Biên trên (của Low Range 1-100,000): 99,999, 100,000, 100,001
        /// - 99,999: Dưới biên (valid - vẫn trong Low Range)
        /// - 100,000: Tại biên (valid - cuối Low Range)
        /// - 100,001: Trên biên (valid - đầu Mid Range)
        /// </summary>
        [Theory]
        [InlineData(99999, true)]   // BVA: Dưới biên trên Low Range
        [InlineData(100000, true)]  // BVA: Tại biên trên Low Range
        [InlineData(100001, true)]  // BVA: Trên biên (chuyển sang Mid Range)
        public async Task CreateAsync_BVA_GiaBanUpperBoundaryLowRange_Success(
            decimal giaBan,
            bool shouldSucceed)
        {
            // Arrange
            var createDto = new CreateThuocDto
            {
                TenThuoc = "Aspirin 100mg",
                GiaBan = giaBan,
                DonVi = "Viên",
                IddanhMuc = 1
            };

            SetupSuccessfulCreate(createDto, giaBan);

            // Act
            var result = await _service.CreateAsync(createDto);

            // Assert
            result.Should().NotBeNull();
            result.GiaBan.Should().Be(giaBan);
        }

        #endregion

        #region BOUNDARY VALUE ANALYSIS - Giá Bán (High Range: 1M - 10M)

        /// <summary>
        /// BVA Test Case 3: Lower Boundary của Valid High Range
        /// 
        /// Biên dưới (của High Range 1M-10M): 999,999, 1,000,000, 1,000,001
        /// - 999,999: Dưới biên (valid - cuối Mid Range)
        /// - 1,000,000: Tại biên (valid - cuối Mid Range)
        /// - 1,000,001: Trên biên (valid - đầu High Range)
        /// </summary>
        [Theory]
        [InlineData(999999, true)]    // BVA: Dưới biên High Range
        [InlineData(1000000, true)]   // BVA: Tại biên
        [InlineData(1000001, true)]   // BVA: Trên biên (đầu High Range)
        public async Task CreateAsync_BVA_GiaBanLowerBoundaryHighRange_Success(
            decimal giaBan,
            bool shouldSucceed)
        {
            // Arrange
            var createDto = new CreateThuocDto
            {
                TenThuoc = "Thuốc cao cấp",
                GiaBan = giaBan,
                DonVi = "Lọ",
                IddanhMuc = 1
            };

            SetupSuccessfulCreate(createDto, giaBan);

            // Act
            var result = await _service.CreateAsync(createDto);

            // Assert
            result.Should().NotBeNull();
            result.GiaBan.Should().Be(giaBan);
        }

        /// <summary>
        /// BVA Test Case 4: Upper Boundary của Valid High Range
        /// 
        /// Biên trên: 9,999,999, 10,000,000, 10,000,001
        /// - 9,999,999: Dưới biên (valid)
        /// - 10,000,000: Tại biên hợp lệ (valid)
        /// - 10,000,001: Vượt biên (invalid)
        /// </summary>
        [Theory]
        [InlineData(9999999, true)]    // BVA: Dưới biên trên
        [InlineData(10000000, true)]   // BVA: Tại biên trên hợp lệ
        [InlineData(10000001, false)]  // BVA: Vượt biên (invalid)
        public async Task CreateAsync_BVA_GiaBanUpperBoundaryHighRange_ReturnsExpectedResult(
            decimal giaBan,
            bool shouldSucceed)
        {
            // Arrange
            var createDto = new CreateThuocDto
            {
                TenThuoc = "Thuốc cao cấp nhất",
                GiaBan = giaBan,
                DonVi = "Lọ",
                IddanhMuc = 1
            };

            if (shouldSucceed)
            {
                SetupSuccessfulCreate(createDto, giaBan);
            }

            // Act & Assert
            if (shouldSucceed)
            {
                var result = await _service.CreateAsync(createDto);
                result.Should().NotBeNull();
                result.GiaBan.Should().Be(giaBan);
            }
            else
            {
                await Assert.ThrowsAsync<BadRequestException>(
                    async () => await _service.CreateAsync(createDto)
                );
            }
        }

        #endregion

        #region EQUIVALENCE PARTITIONING - Tên Thuốc

        /// <summary>
        /// EP Test Case 6: Invalid Partition (Tên thuốc null/empty)
        /// 
        /// Partition: Null, Empty, Whitespace
        /// Kỳ vọng: BadRequestException
        /// </summary>
        [Theory]
        [InlineData(null)]      // EP: Null
        [InlineData("")]        // EP: Empty
        [InlineData("   ")]     // EP: Whitespace
        public async Task CreateAsync_EP_TenThuocInvalid_ThrowsBadRequestException(string? tenThuoc)
        {
            // Arrange
            var createDto = new CreateThuocDto
            {
                TenThuoc = tenThuoc,
                GiaBan = 5000,
                DonVi = "Viên",
                IddanhMuc = 1
            };

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(
                async () => await _service.CreateAsync(createDto)
            );
        }

        /// <summary>
        /// EP Test Case 7: Valid Partition (Tên thuốc hợp lệ)
        /// 
        /// Partition: 1-100 ký tự, chứa chữ, số, khoảng trắng
        /// Đại diện: Tên ngắn, tên dài, có số, có khoảng trắng
        /// Kỳ vọng: Success
        /// </summary>
        [Theory]
        [InlineData("A")]                        // EP: Tên ngắn nhất (1 ký tự)
        [InlineData("Paracetamol")]             // EP: Tên thông thường
        [InlineData("Paracetamol 500mg")]       // EP: Có số và khoảng trắng
        [InlineData("Thuốc giảm đau")]          // EP: Tiếng Việt có dấu
        [InlineData("Aspirin-C")]               // EP: Có dấu gạch nối
        public async Task CreateAsync_EP_TenThuocValid_Success(string tenThuoc)
        {
            // Arrange
            var createDto = new CreateThuocDto
            {
                TenThuoc = tenThuoc,
                GiaBan = 5000,
                DonVi = "Viên",
                IddanhMuc = 1
            };

            SetupSuccessfulCreate(createDto, 5000);

            // Act
            var result = await _service.CreateAsync(createDto);

            // Assert
            result.Should().NotBeNull();
            result.TenThuoc.Should().Be(tenThuoc);
        }

        #endregion

        #region BOUNDARY VALUE ANALYSIS - Độ Dài Tên Thuốc

        /// <summary>
        /// BVA Test Case 5: Boundary của độ dài tên thuốc
        /// 
        /// Giới hạn: 1-100 ký tự
        /// Biên dưới: 0, 1, 2
        /// Biên trên: 99, 100, 101
        /// </summary>
        [Theory]
        [InlineData(0, false)]    // BVA: 0 ký tự (empty - invalid)
        [InlineData(1, true)]     // BVA: 1 ký tự (biên dưới hợp lệ)
        [InlineData(2, true)]     // BVA: 2 ký tự (trên biên dưới)
        [InlineData(50, true)]    // BVA: Giữa range
        [InlineData(99, true)]    // BVA: Dưới biên trên
        [InlineData(100, true)]   // BVA: Tại biên trên hợp lệ
        [InlineData(101, false)]  // BVA: Vượt biên trên (invalid)
        public async Task CreateAsync_BVA_TenThuocLength_ReturnsExpectedResult(
            int length,
            bool shouldSucceed)
        {
            // Arrange
            var tenThuoc = length > 0 ? new string('A', length) : string.Empty;
            var createDto = new CreateThuocDto
            {
                TenThuoc = tenThuoc,
                GiaBan = 5000,
                DonVi = "Viên",
                IddanhMuc = 1
            };

            if (shouldSucceed)
            {
                SetupSuccessfulCreate(createDto, 5000);
            }

            // Act & Assert
            if (shouldSucceed)
            {
                var result = await _service.CreateAsync(createDto);
                result.Should().NotBeNull();
                result.TenThuoc.Should().HaveLength(length);
            }
            else
            {
                await Assert.ThrowsAsync<BadRequestException>(
                    async () => await _service.CreateAsync(createDto)
                );
            }
        }

        #endregion

        #region DECISION TABLE TESTING - Tạo Thuốc Mới

        /// <summary>
        /// Decision Table: CreateAsync
        /// 
        /// Điều kiện:
        /// C1: Tên thuốc hợp lệ (T/F)
        /// C2: Giá bán hợp lệ (T/F)
        /// C3: Danh mục tồn tại (T/F)
        /// 
        /// Kết quả:
        /// R1: Tạo thuốc thành công (T/F)
        /// 
        /// Decision Table:
        /// ┌──────┬────┬────┬────┬────┬─────────────────────────┐
        /// │ Rule │ C1 │ C2 │ C3 │ R1 │ Test Case               │
        /// ├──────┼────┼────┼────┼────┼─────────────────────────┤
        /// │  1   │ T  │ T  │ T  │ T  │ AllValid                │
        /// │  2   │ F  │ T  │ T  │ F  │ InvalidTenThuoc         │
        /// │  3   │ T  │ F  │ T  │ F  │ InvalidGiaBan           │
        /// │  4   │ T  │ T  │ F  │ F  │ DanhMucNotFound         │
        /// │  5   │ F  │ F  │ T  │ F  │ InvalidTenAndGia        │
        /// │  6   │ F  │ T  │ F  │ F  │ InvalidTenAndDanhMuc    │
        /// │  7   │ T  │ F  │ F  │ F  │ InvalidGiaAndDanhMuc    │
        /// │  8   │ F  │ F  │ F  │ F  │ AllInvalid              │
        /// └──────┴────┴────┴────┴────┴─────────────────────────┘
        /// </summary>

        /// <summary>
        /// DT Rule 1: Tất cả điều kiện hợp lệ → Success
        /// C1=T, C2=T, C3=T → R1=T
        /// </summary>
        [Fact]
        public async Task CreateAsync_DT_Rule1_AllValid_Success()
        {
            // Arrange - Tất cả điều kiện TRUE
            var createDto = new CreateThuocDto
            {
                TenThuoc = "Paracetamol 500mg",  // C1: Valid
                GiaBan = 5000,                    // C2: Valid
                DonVi = "Viên",
                IddanhMuc = 1                     // C3: Tồn tại
            };

            SetupSuccessfulCreate(createDto, 5000);

            // Act
            var result = await _service.CreateAsync(createDto);

            // Assert - R1: Success
            result.Should().NotBeNull();
            result.TenThuoc.Should().Be("Paracetamol 500mg");
            result.GiaBan.Should().Be(5000);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        /// <summary>
        /// DT Rule 2: Tên thuốc không hợp lệ → Exception
        /// C1=F, C2=T, C3=T → R1=F
        /// </summary>
        [Fact]
        public async Task CreateAsync_DT_Rule2_InvalidTenThuoc_ThrowsException()
        {
            // Arrange - Chỉ C1 FALSE
            var createDto = new CreateThuocDto
            {
                TenThuoc = "",        // C1: Invalid (empty)
                GiaBan = 5000,        // C2: Valid
                DonVi = "Viên",
                IddanhMuc = 1         // C3: Valid
            };

            // Setup danh mục tồn tại
            _danhMucRepoMock.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(new DanhMuc { Id = 1, TrangThai = true });

            // Act & Assert - R1: Exception
            await Assert.ThrowsAsync<BadRequestException>(
                async () => await _service.CreateAsync(createDto)
            );
        }

        /// <summary>
        /// DT Rule 3: Giá bán không hợp lệ → Exception
        /// C1=T, C2=F, C3=T → R1=F
        /// </summary>
        [Fact]
        public async Task CreateAsync_DT_Rule3_InvalidGiaBan_ThrowsException()
        {
            // Arrange - Chỉ C2 FALSE
            var createDto = new CreateThuocDto
            {
                TenThuoc = "Paracetamol",  // C1: Valid
                GiaBan = 0,                 // C2: Invalid (≤ 0)
                DonVi = "Viên",
                IddanhMuc = 1               // C3: Valid
            };

            // Setup danh mục tồn tại
            _danhMucRepoMock.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(new DanhMuc { Id = 1, TrangThai = true });

            // Act & Assert - R1: Exception
            await Assert.ThrowsAsync<BadRequestException>(
                async () => await _service.CreateAsync(createDto)
            );
        }

        /// <summary>
        /// DT Rule 4: Danh mục không tồn tại → Exception
        /// C1=T, C2=T, C3=F → R1=F
        /// </summary>
        [Fact]
        public async Task CreateAsync_DT_Rule4_DanhMucNotFound_ThrowsException()
        {
            // Arrange - Chỉ C3 FALSE
            var createDto = new CreateThuocDto
            {
                TenThuoc = "Paracetamol",  // C1: Valid
                GiaBan = 5000,              // C2: Valid
                DonVi = "Viên",
                IddanhMuc = 999             // C3: Không tồn tại
            };

            // Setup danh mục KHÔNG tồn tại
            _danhMucRepoMock.Setup(r => r.GetByIdAsync(999))
                .ReturnsAsync((DanhMuc?)null);

            // Act & Assert - R1: Exception
            await Assert.ThrowsAsync<NotFoundException>(
                async () => await _service.CreateAsync(createDto)
            );
        }

        /// <summary>
        /// DT Rule 5: Tên và Giá đều không hợp lệ → Exception
        /// C1=F, C2=F, C3=T → R1=F
        /// </summary>
        [Fact]
        public async Task CreateAsync_DT_Rule5_InvalidTenAndGia_ThrowsException()
        {
            // Arrange - C1=F, C2=F, C3=T
            var createDto = new CreateThuocDto
            {
                TenThuoc = "",     // C1: Invalid
                GiaBan = -1000,    // C2: Invalid
                DonVi = "Viên",
                IddanhMuc = 1      // C3: Valid
            };

            // Act & Assert - R1: Exception (fail ở validation đầu tiên)
            await Assert.ThrowsAsync<BadRequestException>(
                async () => await _service.CreateAsync(createDto)
            );
        }

        /// <summary>
        /// DT Rule 6: Tên không hợp lệ và Danh mục không tồn tại → Exception
        /// C1=F, C2=T, C3=F → R1=F
        /// </summary>
        [Fact]
        public async Task CreateAsync_DT_Rule6_InvalidTenAndDanhMuc_ThrowsException()
        {
            // Arrange - C1=F, C2=T, C3=F
            var createDto = new CreateThuocDto
            {
                TenThuoc = null,   // C1: Invalid
                GiaBan = 5000,     // C2: Valid
                DonVi = "Viên",
                IddanhMuc = 999    // C3: Không tồn tại
            };

            // Act & Assert - R1: Exception
            await Assert.ThrowsAsync<BadRequestException>(
                async () => await _service.CreateAsync(createDto)
            );
        }

        /// <summary>
        /// DT Rule 7: Giá không hợp lệ và Danh mục không tồn tại → Exception
        /// C1=T, C2=F, C3=F → R1=F
        /// </summary>
        [Fact]
        public async Task CreateAsync_DT_Rule7_InvalidGiaAndDanhMuc_ThrowsException()
        {
            // Arrange - C1=T, C2=F, C3=F
            var createDto = new CreateThuocDto
            {
                TenThuoc = "Paracetamol",  // C1: Valid
                GiaBan = 0,                 // C2: Invalid
                DonVi = "Viên",
                IddanhMuc = 999             // C3: Không tồn tại
            };

            // Act & Assert - R1: Exception
            await Assert.ThrowsAsync<BadRequestException>(
                async () => await _service.CreateAsync(createDto)
            );
        }

        /// <summary>
        /// DT Rule 8: Tất cả điều kiện đều không hợp lệ → Exception
        /// C1=F, C2=F, C3=F → R1=F
        /// </summary>
        [Fact]
        public async Task CreateAsync_DT_Rule8_AllInvalid_ThrowsException()
        {
            // Arrange - Tất cả điều kiện FALSE
            var createDto = new CreateThuocDto
            {
                TenThuoc = "",     // C1: Invalid
                GiaBan = -1000,    // C2: Invalid
                DonVi = "Viên",
                IddanhMuc = 999    // C3: Không tồn tại
            };

            // Act & Assert - R1: Exception
            await Assert.ThrowsAsync<BadRequestException>(
                async () => await _service.CreateAsync(createDto)
            );
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Setup cho test case thành công
        /// </summary>
        private void SetupSuccessfulCreate(CreateThuocDto createDto, decimal giaBan)
        {
            // Setup mapper: CreateDto -> Entity
            var thuoc = new Thuoc
            {
                Id = 0,
                TenThuoc = createDto.TenThuoc,
                GiaBan = giaBan,
                DonVi = createDto.DonVi,
                IddanhMuc = createDto.IddanhMuc,
                TrangThai = true
            };

            _mapperMock.Setup(m => m.Map<Thuoc>(createDto))
                .Returns(thuoc);

            // Setup repository: Create trả về entity có ID
            var createdThuoc = new Thuoc
            {
                Id = 1,
                TenThuoc = createDto.TenThuoc,
                GiaBan = giaBan,
                DonVi = createDto.DonVi,
                TrangThai = true
            };

            _thuocRepoMock.Setup(r => r.CreateAsync(It.IsAny<Thuoc>()))
                .ReturnsAsync(createdThuoc);

            // Setup mapper: Entity -> Dto
            var thuocDto = new ThuocDto
            {
                Id = 1,
                TenThuoc = createDto.TenThuoc,
                GiaBan = giaBan,
                DonVi = createDto.DonVi
            };

            _mapperMock.Setup(m => m.Map<ThuocDto>(It.IsAny<Thuoc>()))
                .Returns(thuocDto);

            // Setup danh mục tồn tại
            if (createDto.IddanhMuc.HasValue)
            {
                _danhMucRepoMock.Setup(r => r.GetByIdAsync(createDto.IddanhMuc.Value))
                    .ReturnsAsync(new DanhMuc
                    {
                        Id = createDto.IddanhMuc.Value,
                        TrangThai = true
                    });
            }
        }

        #endregion
    }
}