using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IPCamera.UI.Maps
{

    public partial class MapsEdit : Form
    {
        string FilePath;
        MapsFile maps = new MapsFile();

        MapsOType add;

        public MapsEdit()
        {
            InitializeComponent();
        }
        public MapsEdit(string mapsfile) : this()
        {
            FilePath = mapsfile;
            if (File.Exists(mapsfile))
                maps = new MapsFile(mapsfile);
            else
                maps.FilePath = mapsfile;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            OpenFileDialog opg = new OpenFileDialog()
            {
                Title = "Открыть фоновое изображение карты",
                Filter = "Image Files(*.BMP;*.JPG;*.PNG)|*.BMP;*.JPG;*.PNG",
            };
            if (opg.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = Image.FromFile(opg.FileName);
                maps.UploadFile(opg.FileName);
                maps.Manifest.MainImage = Path.GetFileName(opg.FileName);
            }
        }

        void LoadCameras()
        {
            foreach (var item in pictureBox1.Controls)
            {
                if (item is PictureBox pb)
                    pictureBox1.Controls.Remove(pb);
            }

            foreach (var item in maps.Manifest.Objects)
            {
                var pp = new PictureBox()
                {
                    Location = item.Point,
                    Size = new Size(32, 32),
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Name = "camera_"+item.Name,                   
                    Image = maps.LoadImage(item.Image)
                };
                pp.MouseClick += pictureBox1_MouseClick;
                pp.MouseDoubleClick += Pp_MouseDoubleClick;
                toolTip1.SetToolTip(pp, item.Name);
                pictureBox1.Controls.Add(pp);
            }           
        }

        private void Pp_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var pp = sender as PictureBox;
            var name = pp.Name.Split('_').LastOrDefault();
            var l = maps.Manifest.Objects.ToList();
            var id = l.IndexOf(l.Where(t => t.Name == name).FirstOrDefault());
            var s = new MapsObjectSetting() { TypeObject = 0, NameObject = l[id].Name, Files = maps.GetAllImages() };
            s.Camera = l[id].OID; s.SelectFile = l[id].Image;
            var d = s.ShowDialog();
            
            if (d == DialogResult.OK)
            {
                maps.UploadFile(s.ToLoadFile);
                l[id].Name = s.NameObject;
                l[id].OID = s.Camera;
                l[id].Image = s.SelectFile;
            }
            maps.Manifest.Objects = l.ToArray();
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            maps.Save();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            add = MapsOType.Camera;
        }

        private void MapsEdit_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = maps.LoadMainImage();
            LoadCameras();
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if(add == MapsOType.Camera && (sender is PictureBox pb) && !pb.Name.Contains("camera"))
            {
                add = MapsOType.None;
                var l = maps.Manifest.Objects.ToList();
                var s = new MapsObjectSetting() { TypeObject = 0, Files = maps.GetAllImages() };
                if(s.ShowDialog() == DialogResult.OK)
                {
                    maps.UploadFile(s.ToLoadFile);
                    var obj = new MapsObject(s.NameObject, e.Location, (int)MapsOType.Camera, s.Camera, s.SelectFile);                   
                    l.Add(obj);                  
                }
                maps.Manifest.Objects = l.ToArray();
                //maps.Save();
                LoadCameras();
            }
            else if (add == MapsOType.Delete)
            {
                add = MapsOType.None;
                var l = maps.Manifest.Objects.ToList();
                if ((sender is PictureBox pb1) && pb1.Name.Contains("camera"))
                {
                    var od = l.Where(t => t.Name == pb1.Name.Split('_')[1]).First();
                    l.Remove(od);
                }                             
                maps.Manifest.Objects = l.ToArray();
                //maps.Save();
                LoadCameras();
            }
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            add = MapsOType.Delete;
        }
    }
}
