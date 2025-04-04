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

    public partial class Package
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Package()
        {
            this.Linehaul_Package = new HashSet<Linehaul_Package>();
            this.Package_Order = new HashSet<Package_Order>();
        }
        public Package(System.DateTime time, string statusID, double weight, string packer, string send, string receive)
        {
            this.CreateTime = time;
            this.NumberOfOrder = 0;
            this.StatusID = statusID;
            this.TotalWeight = weight;
            this.Packer = packer;
            this.SendingStation = send;
            this.ReceivingStation = receive;
        }

        [Key]
        [DisplayName("Mã kiện hàng")]
        [Required(ErrorMessage = "Mục này không được để trống!")]
        [StringLength(15, ErrorMessage = "Mục này không được nhập quá 15 ký tự")]
        public string PackageID { get; set; }

        [DisplayName("TG tạo")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm:ss}")]
        [Required(ErrorMessage = "Mục này không được để trống!")]
        public System.DateTime CreateTime { get; set; }

        [DisplayName("TG hoàn thành")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm:ss}")]
        public Nullable<System.DateTime> CompleteTime { get; set; }

        [DisplayName("Số đơn")]
        [Required(ErrorMessage = "Mục này không được để trống!")]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int NumberOfOrder { get; set; }

        [DisplayName("Trạng thái")]
        [Required(ErrorMessage = "Mục này không được để trống!")]
        [StringLength(10, ErrorMessage = "Mục này không được nhập quá 10 ký tự")]
        public string StatusID { get; set; }

        [DisplayName("Tổng KL")]
        [DisplayFormat(DataFormatString = "{0: ## ### ##0.## (kg)}")]
        [Required(ErrorMessage = "Mục này không được để trống!")]
        [Range(0.0, double.MaxValue, ErrorMessage = "Khối lượng tối thiểu là 0.0kg")]
        public double TotalWeight { get; set; }

        [DisplayName("NV đóng")]
        [Required(ErrorMessage = "Mục này không được để trống!")]
        [StringLength(10, ErrorMessage = "Mục này không được nhập quá 10 ký tự")]
        public string Packer { get; set; }

        [DisplayName("Trạm gửi")]
        [Required(ErrorMessage = "Mục này không được để trống!")]
        [StringLength(4)]
        public string SendingStation { get; set; }

        [DisplayName("Trạm nhận")]
        [Required(ErrorMessage = "Mục này không được để trống!")]
        [StringLength(4)]
        public string ReceivingStation { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Linehaul_Package> Linehaul_Package { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Package_Order> Package_Order { get; set; }
        public virtual Staff Staff { get; set; }
        public virtual Station Station { get; set; }
        public virtual Station Station1 { get; set; }
        public virtual Status Status { get; set; }
    }
}
