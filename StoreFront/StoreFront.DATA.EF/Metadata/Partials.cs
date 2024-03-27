using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;//Gives us the ability to annotate all of our data properties
using Microsoft.AspNetCore.Mvc;

namespace StoreFront.DATA.EF.Models//Metadata
{
    //internal class Partials
    //{
    //}

    [ModelMetadataType(typeof(CategoryMetadata))]
    public partial class Category { }


    [ModelMetadataType(typeof(CustomerDetailMetadata))]
    public partial class CustomerDetail { }


    [ModelMetadataType(typeof(OrderMetadata))]
    public partial class Order { }


    [ModelMetadataType(typeof(ProductMetadata))]
    public partial class Product { }

}
