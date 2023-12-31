﻿using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPTUMerchAPI.Model
{
    [FirestoreData]
    public class Orders
    {
        [Key] public string? OrderID { get; set; }
        [FirestoreProperty]
        [ForeignKey("DiscountCode")] public string? DiscountCodeID { get; set; }
        [FirestoreProperty]
        public string OrdererName { get; set; }
        [FirestoreProperty]
        public string OrdererPhoneNumber { get; set; }
        [FirestoreProperty]
        public string OrdererEmail { get; set; }
        [FirestoreProperty]
        public string DeliveryAddress { get; set; }
        [FirestoreProperty]
        public float? TotalPrice { get; set; }
        [FirestoreProperty]
        public DateTime? CreateDate { get; set; }
        [FirestoreProperty]
        public DateTime? UpdateDate { get; set; }
        [FirestoreProperty]
        public string? Note { get; set; }
        [FirestoreProperty]
        //Hình thức nhận hàng: 1. Tại FPT, 2: Shipping
        public int EarningMethod { get; set; }
        [FirestoreProperty]
        //Hình thức thanh toán: 1. Momo, 2: Chuyển khoản ngân hàng
        public int? Payments { get; set; }
        [FirestoreProperty]
        //Tình trạng đơn hàng: 1: Đang xác thực, 2: Đã xác nhận, 3: Đã giao hàng, 4: Huỷ đơn
        public int? Status { get; set; }
        [FirestoreProperty]
        //TRUE: Already Paid, FALSE: Not Paid
        public bool? PaidStatus { get; set; }
        [FirestoreProperty]
        public string? Shipper { get; set; }
        [FirestoreProperty]
        public List<OrderDetail> orderDetails { get; set; }
    }
}
