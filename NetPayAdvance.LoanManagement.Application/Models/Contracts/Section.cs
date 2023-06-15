using System.Collections.Generic;

namespace NetPayAdvance.LoanManagement.Application.Models.Contracts
{
    public class Section
    {
        public int SectionID { get; set; }
        
        public string Title { get; set; }
        
        public string FilePath { get; set; }
        
        public bool IsOptional { get; set; }
        
        public int ConstructOrder { get; set; }
        
        public int SignOrder { get; set; }
        
        public bool IsChecked { get; set; }
        
        public string Contract { get; set; }
        
        public string SectionType { get; set; }

        public List<SectionOption> Options { get; set; }
    }

    public class SectionOption
    {
        public string Control { get; set; }
     
        public string CheckedControl { get; set; }
        
        public bool IsChecked { get; set; }
    }
}
