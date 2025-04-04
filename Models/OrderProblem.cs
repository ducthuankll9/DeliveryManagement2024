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

    public partial class OrderProblem
    {
        [Key]
        [DisplayName("Mã đơn")]
        [Required(ErrorMessage = "Mục này không được để trống!")]
        [Column(Order = 0)]
        [StringLength(15, ErrorMessage = "Mục này không được nhập quá 15 ký tự")]
        public string OrderID { get; set; }

        [Key]
        [DisplayName("TG cập nhật")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm:ss}")]
        [Required(ErrorMessage = "Mục này không được để trống!")]
        [Column(Order = 1)]
        public System.DateTime UpdateTime { get; set; }

        [DisplayName("Tình trạng")]
        [Required(ErrorMessage = "Mục này không được để trống!")]
        [StringLength(1000, ErrorMessage = "Mục này không được nhập quá 1000 ký tự")]
        public string Status { get; set; }

        [DisplayName("Trạng thái xử lý")]
        [Required(ErrorMessage = "Mục này không được để trống!")]
        public bool IsDisposed { get; set; }

        [DisplayName("Khả năng tiếp tục")]
        [Required(ErrorMessage = "Mục này không được để trống!")]
        public bool Continuable { get; set; }

        [DisplayName("NV báo cáo")]
        [Required(ErrorMessage = "Mục này không được để trống!")]
        [StringLength(10, ErrorMessage = "Mục này không được nhập quá 10 ký tự")]
        public string ReportStaff { get; set; }

        [DisplayName("NV xử lý")]
        [Required(ErrorMessage = "Mục này không được để trống!")]
        [StringLength(10, ErrorMessage = "Mục này không được nhập quá 10 ký tự")]
        public string DisposedStaff { get; set; }
    
        public virtual Order Order { get; set; }
        public virtual Staff Staff { get; set; }
        public virtual Staff Staff1 { get; set; }
    }
}
