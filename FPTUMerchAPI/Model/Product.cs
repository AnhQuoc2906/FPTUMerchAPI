using Google.Cloud.Firestore;
using Google.Rpc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPTUMerchAPI.Model
{
    [FirestoreData]
    public class Product
    {
        public string? ProductID { get; set; }
        [FirestoreProperty]
        public string ProductName { get; set; }
        [FirestoreProperty]
        public string? ProductDescription { get; set; }
        [FirestoreProperty]
        public string ProductLink { get; set; }
        [FirestoreProperty]
        public float Price { get; set; }
        [FirestoreProperty]
        public int Quantity { get; set; } // Số lượng hàng ban đầu
        [FirestoreProperty]
        public int ProductType { get; set; } // Loại hàng: 1: Hàng lẻ, 2: Hàng combo
        [FirestoreProperty]
        public bool? IsActive { get; set; } // TRUE: Đang bán, FALSE: Không bán
        [FirestoreProperty]
        public string? Note { get; set; }
    }
}
