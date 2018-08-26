using System;

namespace DP_EX1
{
     public class SortingAttribute : Attribute
     {
          public string Name { get; set; }


          public SortingAttribute(string i_Name)
          {
               Name = i_Name;
          }


          public override string ToString()
          {
               return this.Name;
          }
     }
}
