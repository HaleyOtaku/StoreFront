using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;//Gives us the ability to annotate all of our data properties

namespace StoreFront.DATA.EF.Models//Metadata
{
    //internal class Metadata
    //{
    //}

    public class CategoryMetadata 
    {
        //PRIMARY KEY: NO ANNOTATIONS
        public int CategoryId { get; set; }

        [StringLength(50)]
        [Display(Name = "Category")]
        [Required]
        public string CategoryName { get; set; } = null!;

        [StringLength(4000)]
        [Display(Name = "Description")]
        public string? CategoryDescript { get; set; }
    }

    public class CustomerDetailMetadata 
    {
        //PRIMARY KEY: NO ANNOTATIONS
        public string CustomerId { get; set; } = null!;


        [Display(Name = "First Name")]
        [DataType(DataType.Text)]
        [StringLength(20)]
        [Required]
        public string FirstName { get; set; } = null!;


        [Display(Name = "Last Name")]
        [DataType(DataType.Text)]
        [StringLength(20)]
        [Required]
        public string LastName { get; set; } = null!;



        [StringLength(50)]
        [Required]
        public string Address { get; set; } = null!;



        [DataType(DataType.Text)]
        [StringLength(10)]
        [Required]
        public string City { get; set; } = null!;



        [DataType(DataType.Text)]
        [StringLength(2)]
        [Required]
        public string State { get; set; } = null!;


        [DataType(DataType.PostalCode)]
        [StringLength(5)]
        [Required]
        public string Zip { get; set; } = null!;


        [Display(Name = "Phone Number")]
        [DataType(DataType.PhoneNumber)]
        [StringLength(10)]
        public string? Phone { get; set; }
    }

    public class OrderMetadata 
    {
        //PRIMARY KEY: NO ANNOTATIONS
        public int OrderId { get; set; }


        //FOREIGN KEY: NO ANNOTATIONS
        public string CustomerId { get; set; } = null!;



        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        [Display(Name = "Order Date")]
        [Required]
        public DateTime OrderDate { get; set; }

        [Display(Name = "Ship To")]
        [StringLength(50)]
        [Required]
        public string ShipToName { get; set; } = null!;


        [Display(Name = "City")]
        [StringLength(10)]
        [Required]
        public string ShipCity { get; set; } = null!;


        [Display(Name = "State")]
        [StringLength(2)]
        [Required]
        public string ShipState { get; set; } = null!;


        [Display(Name = "Postal Code")]
        [DataType(DataType.PostalCode)]
        [StringLength(5)]
        [Required]
        public string ShipZip { get; set; } = null!;
    }

    public class ProductMetadata 
    {
        //PRIMARY KEY: NO ANNOTATIONS
        public int ProductId { get; set; }



        [Display(Name = "Gadget")]
        [StringLength(50)]
        [Required]
        public string ProductName { get; set; } = null!;


        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:c}")]
        [Display(Name = "Price")]
        [Range(0, (double)decimal.MaxValue)]
        [Required]
        public decimal ProductPrice { get; set; }


        [Display(Name = "Description")]
        [UIHint("MultilineText")]
        [DataType(DataType.MultilineText)]
        [StringLength(4000)]
        public string? ProductDescript { get; set; }



        [Display(Name = "In Stock")]
        [Range(0, short.MaxValue)]
        [Required]
        public short UnitsInStock { get; set; }


        //FOREIGN KEY: NO ANNOTATIONS
        public int CategoryId { get; set; }


        //FOREIGN KEY: NO ANNOTATIONS
        public int ProductStatusId { get; set; }






        public string? ProductImage { get; set; }
    }

}
