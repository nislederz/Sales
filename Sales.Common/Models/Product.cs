namespace Sales.Common.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        [Required]
        [StringLength(50)]
        public string Description { get; set; }
        [DataType (DataType.MultilineText)]
        public string Remarks { get; set; }
        [Display(Name ="Image")]
        public string ImagePath { get; set; }
        [DisplayFormat(DataFormatString ="{0:C2}",ApplyFormatInEditMode =false)]
        public Decimal Price { get; set; }
        [Display(Name = "Is Avalible")]
        public bool IsAvailable { get; set; }
        [Display(Name = "Publis On")]
        [DataType(DataType.Date)]
        public DateTime PublishOn { get; set; }
        public override string ToString()
        {
            return this.Description;
        }
    }
}
