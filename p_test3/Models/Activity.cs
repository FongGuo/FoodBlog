//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace p_test3.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Activity
    {
        public int ActivityId { get; set; }
        public string DateTime { get; set; }
        public string Location { get; set; }
        public string PersonLimitation { get; set; }
        public string Paragraphy { get; set; }
        public string Title { get; set; }
        public string UserName { get; set; }
        public string ServerPath { get; set; }
        public Nullable<int> Participation { get; set; }
        public Nullable<int> UserId { get; set; }
    
        public virtual User User { get; set; }
    }
}
