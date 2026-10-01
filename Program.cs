using System;
using System.Collections.Generic;
using System.Linq;

// A. Abstract class PhuongTien
abstract class PhuongTien
{
    private string _maPT;
    private string _tenHang;
    private int _namSanXuat;
    private decimal _giaGoc;

    public string MaPT
    {
        get { return _maPT; }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("MaPT khong duoc de trong!");
            _maPT = value;
        }
    }

    public string TenHang
    {
        get { return _tenHang; }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Ten hang khong duoc de trong!");
            _tenHang = value;
        }
    }

    public int NamSanXuat
    {
        get { return _namSanXuat; }
        set
        {
            if (value < 1900 || value > DateTime.Now.Year)
                throw new ArgumentException("Nam san xuat khong hop le!");
            _namSanXuat = value;
        }
    }

    public decimal GiaGoc
    {
        get { return _giaGoc; }
        set
        {
            if (value <= 0)
                throw new ArgumentException("Gia goc phai lon hon 0!");
            _giaGoc = value;
        }
    }

    // Constructor
    public PhuongTien(string maPT, string tenHang, int namSanXuat, decimal giaGoc)
    {
        MaPT = string.IsNullOrWhiteSpace(maPT) ? "PT0000" : maPT;
        TenHang = tenHang;
        NamSanXuat = namSanXuat;
        GiaGoc = giaGoc;
    }

    // Abstract method
    public abstract decimal TinhGiaLanBanh();

    // Virtual method
    public virtual string GetInfo()
    {
        return $"Ma PT: {MaPT}, Hang: {TenHang}, " +
               $"Nam SX: {NamSanXuat}, Gia goc: {GiaGoc:N0}";
    }
}

// B. Class OTo
class OTo : PhuongTien
{
    public int SoChoNgoi { get; set; }
    public double DungTichDongCo { get; set; }

    public OTo(string maPT, string tenHang, int namSX,
               decimal giaGoc, int soChoNgoi, double dungTich)
        : base(maPT, tenHang, namSX, giaGoc)
    {
        if (soChoNgoi <= 0)
            throw new ArgumentException("So cho ngoi phai > 0!");
        if (dungTich <= 0)
            throw new ArgumentException("Dung tich phai > 0!");

        SoChoNgoi = soChoNgoi;
        DungTichDongCo = dungTich;
    }

    public override decimal TinhGiaLanBanh()
    {
        if (SoChoNgoi <= 9)
            return GiaGoc + GiaGoc * 0.12m + GiaGoc * 0.30m;
        else
            return GiaGoc + GiaGoc * 0.10m;
    }

    public override string GetInfo()
    {
        return base.GetInfo() +
               $", So cho: {SoChoNgoi}, " +
               $"Dung tich: {DungTichDongCo}, " +
               $"Gia lan banh: {TinhGiaLanBanh():N0}";
    }
}

// C. Class XeMay
class XeMay : PhuongTien
{
    public int DungTichXylanh { get; set; }

    public XeMay(string maPT, string tenHang, int namSX,
                  decimal giaGoc, int dungTich)
        : base(maPT, tenHang, namSX, giaGoc)
    {
        if (dungTich <= 0)
            throw new ArgumentException("Dung tich xylanh phai > 0!");

        DungTichXylanh = dungTich;
    }

    public override decimal TinhGiaLanBanh()
    {
        if (DungTichXylanh < 175)
            return GiaGoc + GiaGoc * 0.02m;
        else
            return GiaGoc + GiaGoc * 0.05m;
    }

    public override string GetInfo()
    {
        return base.GetInfo() +
               $", Xylanh: {DungTichXylanh} cc, " +
               $"Gia lan banh: {TinhGiaLanBanh():N0}";
    }
}

// D. Class QuanLyPhuongTien
class QuanLyPhuongTien
{
    private List<PhuongTien> danhSach = new List<PhuongTien>();

    // Them phuong tien
    public void AddPhuongTien(PhuongTien pt)
    {
        if (pt == null)
            throw new ArgumentNullException(nameof(pt));

        danhSach.Add(pt);
    }

    // Hien thi danh sach
    public void DisplayAll()
    {
        if (danhSach.Count == 0)
        {
            Console.WriteLine("Danh sach trong!");
            return;
        }

        foreach (PhuongTien pt in danhSach)
        {
            Console.WriteLine(pt.GetInfo());
        }
    }

    // Tim phuong tien co gia lan banh cao nhat
    public PhuongTien FindMaxGiaLanBanh()
    {
        return danhSach
            .OrderByDescending(pt => pt.TinhGiaLanBanh())
            .FirstOrDefault();
    }

    // Tim theo ten hang bang LINQ
    public List<PhuongTien> SearchByName(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return new List<PhuongTien>();

        return danhSach
            .Where(pt => pt.TenHang.Contains(
                keyword, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }
}

// Main
class Program
{
    static void Main(string[] args)
    {
        QuanLyPhuongTien ql = new QuanLyPhuongTien();

        // Them du lieu mau
        ql.AddPhuongTien(new OTo("PT001", "Toyota", 2022,
            800000000m, 5, 2.0));

        ql.AddPhuongTien(new OTo("PT002", "Ford", 2023,
            1200000000m, 7, 2.5));

        ql.AddPhuongTien(new XeMay("PT003", "Honda", 2024,
            45000000m, 150));

        ql.AddPhuongTien(new XeMay("PT004", "Yamaha", 2023,
            60000000m, 175));

        Console.WriteLine("=== DANH SACH PHUONG TIEN ===");
        ql.DisplayAll();

        Console.WriteLine("\n=== PHUONG TIEN CO GIA LAN BANH CAO NHAT ===");
        PhuongTien max = ql.FindMaxGiaLanBanh();

        if (max != null)
            Console.WriteLine(max.GetInfo());

        Console.WriteLine("\n=== TIM KIEM THEO TEN HANG ===");
        Console.Write("Nhap ten hang can tim: ");
        string keyword = Console.ReadLine();

        List<PhuongTien> ketQua = ql.SearchByName(keyword);

        if (ketQua.Count > 0)
        {
            foreach (PhuongTien pt in ketQua)
                Console.WriteLine(pt.GetInfo());
        }
        else
        {
            Console.WriteLine("Khong tim thay phuong tien!");
        }
    }
}
