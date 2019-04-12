using BrightIdeasSoftware;
using DBClass;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace smartTracker
{
    internal class BusinessCardOverlay : AbstractOverlay
    {
        public static int MaxLineItemCard = 11;

        readonly int nbItem = 5;
        public BusinessCardOverlay(int nbItem)
        {
            businessCardRenderer.HeaderBackBrush = Brushes.DarkBlue;
            businessCardRenderer.BorderPen = new Pen(Color.DarkBlue, 2);
            Transparency = 255;

            if (nbItem <= MaxLineItemCard)
                this.nbItem = nbItem;
            else this.nbItem = MaxLineItemCard;
        }
        #region IOverlay Members

        public override void Draw(ObjectListView olv, Graphics g, Rectangle r)
        {
            if (olv.HotRowIndex < 0)
                return;

            if (olv.View == View.Tile)
                return;

            OLVListItem item = olv.GetItem(olv.HotRowIndex);
            if (item == null)
                return;

            Size cardSize = new Size(320, 100 + (nbItem * 12));
            Rectangle cardBounds = new Rectangle(
                r.Right - cardSize.Width - 8, r.Bottom - cardSize.Height - 8, cardSize.Width, cardSize.Height);
                
            businessCardRenderer.DrawBusinessCard(g, cardBounds, item.RowObject, olv, item);                
        }

        #endregion

        private readonly BusinessCardRenderer businessCardRenderer = new BusinessCardRenderer();
    }
    /// <summary>
    /// Hackish renderer that draw a fancy version of a person for a Tile view.
    /// </summary>
    /// <remarks>This is not the way to write a professional level renderer.
    /// It is hideously inefficient (we should at least cache the images),
    /// but it is obvious</remarks>
    internal class BusinessCardRenderer : AbstractRenderer
    {
        public override bool RenderItem(DrawListViewItemEventArgs e, Graphics g, Rectangle itemBounds, object rowObject)
        {
            // If we're in any other view than Tile, return false to say that we haven't done
            // the rendereing and the default process should do it's stuff
            ObjectListView olv = e.Item.ListView as ObjectListView;
            if (olv == null || olv.View != View.Tile)
                return false;

            // Use buffered graphics to kill flickers
            BufferedGraphics buffered = BufferedGraphicsManager.Current.Allocate(g, itemBounds);
            g = buffered.Graphics;
            g.Clear(olv.BackColor);
            g.SmoothingMode = ObjectListView.SmoothingMode;
            g.TextRenderingHint = ObjectListView.TextRenderingHint;

            if (e.Item.Selected)
            {
                BorderPen = Pens.Blue;
                HeaderBackBrush = new SolidBrush(olv.HighlightBackgroundColorOrDefault);
            }
            else
            {
                BorderPen = new Pen(Color.FromArgb(0x33, 0x33, 0x33));
                HeaderBackBrush = new SolidBrush(Color.FromArgb(0x33, 0x33, 0x33));
            }
            DrawBusinessCard(g, itemBounds, rowObject, olv, (OLVListItem)e.Item);

            // Finally render the buffered graphics
            buffered.Render();
            buffered.Dispose();

            // Return true to say that we've handled the drawing
            return true;
        }

        internal Pen BorderPen = new Pen(Color.FromArgb(0x33, 0x33, 0x33));
        internal Brush TextBrush = new SolidBrush(Color.FromArgb(0x22, 0x22, 0x22));
        internal Brush HeaderTextBrush = Brushes.AliceBlue;
        internal Brush HeaderBackBrush = new SolidBrush(Color.FromArgb(0x33, 0x33, 0x33));
        internal Brush BackBrush = Brushes.LemonChiffon;

        public void DrawBusinessCard(Graphics g, Rectangle itemBounds, object rowObject, ObjectListView olv, OLVListItem item)
        {
            const int spacing = 8;

            // Allow a border around the card
            itemBounds.Inflate(-2, -2);

            // Draw card background
            const int rounding = 20;
            GraphicsPath path = GetRoundedRect(itemBounds, rounding);
            g.FillPath(BackBrush, path);
            g.DrawPath(BorderPen, path);
                
            g.Clip = new Region(itemBounds);
               
            // Draw the photo
            Rectangle photoRect = itemBounds;
            photoRect.Inflate(-spacing, -spacing);

            photoRect.Width = 80;
            OLVColumn columnLotID = olv.GetColumn(2);

            Image photo = getImage(columnLotID.GetStringValue(rowObject));
            if (photo.Width > photoRect.Width)
                photoRect.Height = (int)(photo.Height * ((float)photoRect.Width / photo.Width));
            else
                photoRect.Height = photo.Height;
            g.DrawImage(photo, photoRect);




            // Now draw the text portion
            RectangleF textBoxRect = photoRect;
                
            textBoxRect.X += (photoRect.Width + spacing);
            textBoxRect.Width = itemBounds.Right - textBoxRect.X - spacing;

            StringFormat fmt = new StringFormat(StringFormatFlags.NoWrap);
            fmt.Trimming = StringTrimming.EllipsisCharacter;
            fmt.Alignment = StringAlignment.Center;
            fmt.LineAlignment = StringAlignment.Near;
            //String txt = item.Text;
            String txt = columnLotID.GetStringValue(rowObject);

            using (Font font = new Font("Tahoma", 11))
            {
                // Measure the height of the title
                SizeF size = g.MeasureString(txt, font, (int)textBoxRect.Width, fmt);
                // Draw the title
                RectangleF r3 = textBoxRect;
                r3.Height = size.Height;
                path = GetRoundedRect(r3, 15);
                g.FillPath(HeaderBackBrush, path);
                g.DrawString(txt, font, HeaderTextBrush, textBoxRect, fmt);                    
                textBoxRect.Y += size.Height + spacing;
            }

            // Draw the other bits of information
            using (Font font = new Font("Tahoma", 8))
            {
                SizeF size = g.MeasureString("Wj", font, itemBounds.Width, fmt);
                textBoxRect.Height = size.Height;
                fmt.Alignment = StringAlignment.Near;
                for (int i = 3; i < olv.Columns.Count; i++)
                {
                    OLVColumn column = olv.GetColumn(i);
                    if (column.IsTileViewColumn)
                    {
                        txt = column.AspectName + " : " + column.GetStringValue(rowObject);
                        g.DrawString(txt, font, TextBrush, textBoxRect, fmt);
                        textBoxRect.Y += size.Height;
                    }

                    if (i > BusinessCardOverlay.MaxLineItemCard) break; // stop display other info for high column count
                }
            }
        }


        public static Image getImage(string LotID)
        {

            System.Drawing.Image imgFromDB = Properties.Resources.No_Image_Available;
            MainDBClass dbTemp = new MainDBClass();

            dbTemp.OpenDB();
            if (dbTemp.IsImageLinkExist(LotID))
            {
                string imgName = dbTemp.getImageNameLink(LotID);
                if (imgName != null)
                {
                    imgFromDB = DataClass.ImageUtils.byteArrayToImage(dbTemp.getImage(imgName));
                }
            }
            dbTemp.CloseDB();
            return imgFromDB;
        }


        private GraphicsPath GetRoundedRect(RectangleF rect, float diameter)
        {
            GraphicsPath path = new GraphicsPath();

            RectangleF arc = new RectangleF(rect.X, rect.Y, diameter, diameter);
            path.AddArc(arc, 180, 90);
            arc.X = rect.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = rect.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = rect.Left;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();

            return path;
        }
    }
}
