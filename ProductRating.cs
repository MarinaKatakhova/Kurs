//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WpfApp9
{
    using System;
    using System.Collections.Generic;
    
    public partial class ProductRating
    {
        public int RatingID { get; set; }
        public int ProductID { get; set; }
        public int CustomerID { get; set; }
        public int Rating { get; set; }
        public string Review { get; set; }
    
        public virtual Customer Customer { get; set; }
        public virtual Product Product { get; set; }
    }
}
