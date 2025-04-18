﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DeliveryManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class ShippingRate
    {
        [Key]
        [DisplayName("Trạm gửi")]
        [Required(ErrorMessage = "Mục này không được để trống!")]
        [Column(Order = 0)]
        [StringLength(4)]
        public string SendingStation { get; set; }

        [Key]
        [DisplayName("Trạm nhận")]
        [Required(ErrorMessage = "Mục này không được để trống!")]
        [Column(Order = 1)]
        [StringLength(4)]
        public string ReceivingStation { get; set; }

        [DisplayName("Giá tối thiểu")]
        [DisplayFormat(DataFormatString = "{0: ## ### ##0.## đ}")]
        [Required(ErrorMessage = "Mục này không được để trống!")]
        [Range(1, double.MaxValue, ErrorMessage = "Mục này phải lớn hơn 1")]
        public double MinPrice { get; set; }

        [DisplayName("Giá mỗi Kg")]
        [DisplayFormat(DataFormatString = "{0: ## ### ##0.## đ}")]
        [Required(ErrorMessage = "Mục này không được để trống!")]
        [Range(1, double.MaxValue, ErrorMessage = "Mục này phải lớn hơn 1")]
        public double PricePerKg { get; set; }

        [DisplayName("Giá tối thiểu đơn GTC")]
        [DisplayFormat(DataFormatString = "{0: ## ### ##0.## đ}")]
        [Required(ErrorMessage = "Mục này không được để trống!")]
        [Range(1, double.MaxValue, ErrorMessage = "Mục này phải lớn hơn 1")]
        public Nullable<double> MinPriceForHVO { get; set; }

        [DisplayName("Giá mỗi Kg đơn GTC")]
        [DisplayFormat(DataFormatString = "{0: ## ### ##0.## đ}")]
        [Required(ErrorMessage = "Mục này không được để trống!")]
        [Range(1, double.MaxValue, ErrorMessage = "Mục này phải lớn hơn 1")]
        public Nullable<double> PricePerKgForHVO { get; set; }
    
        public virtual Station Station { get; set; }
        public virtual Station Station1 { get; set; }
    }
}
