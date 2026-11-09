using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoangHaPCVN.Models
{
    public class CartItem
    {
        public string Ten { get; set; }
        public long Gia { get; set; }   
        public string Hinh { get; set; }
        public int SoLuong { get; set; }
        public long ThanhTien
        {
            get { return SoLuong * Gia; }
        }
    }
}