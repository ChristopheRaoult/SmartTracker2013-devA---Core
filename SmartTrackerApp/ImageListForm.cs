using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using DBClass;
using smartTracker.Properties;

namespace smartTracker
{
    public partial class ImageListForm : Form
    {
        MainDBClass _db;
        public string ImageFromList = null;
        public ImageListForm()
        {
            InitializeComponent();
        }

        private void ImageListForm_Load(object sender, EventArgs e)
        {
            _db = new MainDBClass();
            _db.OpenDB();

            RefreshImage();
            
        }

        private void RefreshImage()
        {
            ArrayList imgList = _db.GetImageListName();
            if (imgList != null)
            {
                imageList.Images.Clear();
                listViewImage.Items.Clear();
                imageList.ImageSize = new Size(75, 75);
                int nIndex = 0;
                foreach (string imgName in imgList)
                {
                    int nbUse = _db.getNumberofLink(imgName);
                    imageList.Images.Add(imgName, DataClass.ImageUtils.byteArrayToImage(_db.getImage(imgName)));
                    ListViewItem iListView = listViewImage.Items.Add(string.Format(ResStrings.ImageListForm_RefreshImage_, imgName, nbUse), nIndex++);
                    iListView.ToolTipText = ResStrings.ImageListForm_RefreshImage_Double_Click_to_associate_with_the_image;
                }
            }
            listViewImage.LargeImageList = imageList;
            listViewImage.Refresh();
        }

        private void ImageListForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _db.CloseDB();
        }

        private void toolStripButtonRemove_Click(object sender, EventArgs e)
        {
            if (listViewImage.SelectedItems.Count > 0)
            {
                for (int loop = 0; loop < listViewImage.SelectedItems.Count; loop++)
                {
                    string imgName = listViewImage.SelectedItems[loop].Text;
                    int rg = imgName.IndexOf("\n");
                    imgName = imgName.Substring(0, rg - 1 );
                    _db.DeleteallImageLink(imgName);
                    _db.DeleteImage(imgName);

                }
            }
            RefreshImage();
        }

        private void toolStripButtonRefresh_Click(object sender, EventArgs e)
        {
            RefreshImage();
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void listViewImage_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listViewImage.SelectedItems.Count > 0)
            {
                 string imgName = listViewImage.SelectedItems[0].Text;
                 int rg = imgName.IndexOf("\n");
                 ImageFromList = imgName.Substring(0, rg);
                 Close();                   
            }
        }
    }
}
