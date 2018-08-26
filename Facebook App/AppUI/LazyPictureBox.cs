using System.Windows.Forms;

namespace DP_EX1
{
     public class LazyPictureBox : PictureBox
     {
         public string Url { get; set; }

         public new void Load(string i_UrlToLoad)
          {
               Url = i_UrlToLoad;
          }

          protected override void OnPaint(PaintEventArgs pe)
          {
               if(base.ImageLocation == null)
               {
                    base.ImageLocation = this.Url;
               }
               base.OnPaint(pe);
          }
     }
}
