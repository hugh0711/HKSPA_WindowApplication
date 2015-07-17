using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.IO;
using HtmlAgilityPack;
using System.Drawing.Drawing2D;
using Microsoft.VisualBasic;
using System.Drawing.Imaging;
using System.Globalization;


namespace HKSPA_copy_old_data
{
    public partial class Form1 : Form
    {

        Dictionary<int, string> secondLevel_CategoryID_CategoryName = new Dictionary<int, string>();

        Dictionary<int, string> AlbumID_AlbumName = new Dictionary<int, string>();

        MySqlConnection mysql_connection  = new MySqlConnection("server=localhost;Port=3306; database=hkspa; uid=root; pwd=admin2011;");

        int total_count_row ;

        int range = 10000;

        HKSPADataClassesDataContext HKSPA_ms_db = new HKSPADataClassesDataContext();

        Timer tmr_copyData = new Timer();

        Timer tmr_copyMysql_Data = new Timer();

        //C:\website\HKSPA\product_image\album
        //string new_album_path = @"D:\Project\hkspa\20131211\HKSPA\product_image\album";
        string new_album_path = @"C:\website\HKSPA\product_image\album";


        int rowID;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {


            
        


        }

        protected string GetAlbumName(int album_ID)
        {

            string album_url = string.Format("http://www.hkspa.org/showGallery.php?L=C&albumID={0}", album_ID);

            string return_albumName = "";

            HtmlWeb web = new HtmlWeb
            {
                AutoDetectEncoding = false,
                OverrideEncoding = Encoding.GetEncoding("big5")
            };

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();

            doc = web.Load(album_url);

            return_albumName = doc.DocumentNode.SelectNodes("//font").FindFirst("").InnerText;

            return return_albumName;

        }


        protected void getPhotoDetail(int limit_from, int limit_to)
        {



            txt_result.Text += string.Format("Start copy photos. \r\n");

            //try
            //{


                if (total_count_row <= (limit_to-range))
                {
                    
                    //tmr_copyData.Enabled = false;

                    //txt_result.Text += string.Format("End of program. \r\n");
                }
                else
                {
                    limit_to = total_count_row;

                    //string selectString = string.Format("select * from hkspa.album_view Limit {0},{1}", limit_from, limit_to);

                    //mysql_connection.Open();

                    //dynamic MySqlCommand = mysql_connection.CreateCommand();

                    //MySqlCommand.CommandText = selectString;

                    //MySqlDataReader myReader = null;

                    //myReader = MySqlCommand.ExecuteReader();

                    //DataTable dt_album_view = new DataTable();

                    //dt_album_view.Load(myReader);

                    string album_result = "";


                    Dictionary<string, DateTime> special_album_foldername = new Dictionary<string, DateTime>();

                    special_album_foldername.Add("2003athletics", new DateTime(2013, 12, 24, 0, 0, 0));
                    special_album_foldername.Add("2003badminton", new DateTime(2013, 12, 24, 0, 0, 0));
                    special_album_foldername.Add("2003boccia", new DateTime(2013, 12, 24, 0, 0, 0));
                    special_album_foldername.Add("2003swimming", new DateTime(2013, 12, 24, 0, 0, 0));
                    special_album_foldername.Add("2003table tennis", new DateTime(2013, 12, 24, 0, 0, 0));
                    special_album_foldername.Add("200510", new DateTime(2005, 10, 01, 0, 0, 0));
                    special_album_foldername.Add("200511", new DateTime(2005, 11, 01, 0, 0, 0));
                    special_album_foldername.Add("200512_200601", new DateTime(2005, 12, 01, 0, 0, 0));
                    special_album_foldername.Add("200601-05", new DateTime(2006, 01, 01, 0, 0, 0));
                    special_album_foldername.Add("201209", new DateTime(2012, 09, 01, 0, 0, 0));
                    special_album_foldername.Add("201210", new DateTime(2012, 10, 01, 0, 0, 0));
                    special_album_foldername.Add("201211", new DateTime(2012, 11, 01, 0, 0, 0));
                    special_album_foldername.Add("201212", new DateTime(2012, 12, 01, 0, 0, 0));

                    Dictionary<int, int> skip_id = new Dictionary<int, int>();

                    skip_id.Add(2191, 2191); //2191	Ball Games	ball	Volleyball	volleyball	02-05 Nov 2003 -<br> 2nd Asian Schools Volleyball Championship (Boys) 2003	20031202-05	46	086	Fanling	Nikon D1H	419	640
                    skip_id.Add(2192, 2192);//2192	Ball Games	ball	Volleyball	volleyball	02-05 Nov 2003 -<br> 2nd Asian Schools Volleyball Championship (Boys) 2003	20031202-05	46	087	Fanling	Nikon D1H	640	419
                    skip_id.Add(2193, 2193);//2193	Ball Games	ball	Volleyball	volleyball	02-05 Nov 2003 -<br> 2nd Asian Schools Volleyball Championship (Boys) 2003	20031202-05	46	088	Fanling	Nikon D1H	640	419
                    skip_id.Add(2194, 2194);//2194	Ball Games	ball	Volleyball	volleyball	02-05 Nov 2003 -<br> 2nd Asian Schools Volleyball Championship (Boys) 2003	20031202-05	46	089	Fanling	Nikon D1H	640	419

                    skip_id.Add(2696, 2696);//Image Damage


                    int counter = 0;
                    string line;

                    string[] errorIDList = File.ReadAllLines(Path.Combine(Environment.CurrentDirectory, "Error_ID.txt")); // relative path
                    int skipNumber = 100;
                    //for (int i = 0; i <= errorIDList.Length; i += skipNumber)
                    //{

                        //later check ID < 58521
                        IQueryable<old_album_photo_record> photo_records = (from d in HKSPA_ms_db.old_album_photo_records
                                                                            where d.ID == 161770
                                                                            select d);

                        //foreach (DataRow photo_row in dt_album_view.Rows)
                        foreach (old_album_photo_record photo_row in photo_records)
                        {


                            string filename = "";

                            string author = "";

                            string camera_model = "";

                            //int photo_sortOrder = Convert.ToInt32(photo_row["FileNo"]);

                            //string photo_sortOrder_string = photo_row["FileNo"].ToString();

                            //string sub_category_name=photo_row["sub_CategoryName"].ToString();

                            //if (photo_row["FileName"] != null & photo_row["FileName"].ToString().Length > 0)
                            //{
                            //    author = photo_row["FileName"].ToString();
                            //}

                            //if (photo_row["FileCam"] != null & photo_row["FileCam"].ToString().Length > 0)
                            //{
                            //    camera_model = photo_row["FileCam"].ToString();
                            //}

                            int photo_sortOrder = Convert.ToInt32(photo_row.FileNo);

                            string photo_sortOrder_string = photo_row.FileNo;

                            string sub_category_name = photo_row.sub_CategoryName;

                            if (photo_row.fileName != null & photo_row.fileName.ToString().Length > 0)
                            {
                                author = photo_row.fileName;
                            }

                            if (photo_row.FileCam != null & photo_row.FileCam.ToString().Length > 0)
                            {
                                camera_model = photo_row.FileCam.ToString();
                            }



                            if (author != "" & author.Length > 0 & camera_model != "" & camera_model.Length > 0)
                            {

                                filename = string.Format("_{0}_{1}", author, camera_model);
                            }

                            //http://www.hkspa.org/Gallery/ball/Tennis/2008010205/001_Down%20Ka%20Sing_Canon%20EOS-1D%20II.jpg
                            //string photo_url = string.Format("http://www.hkspa.org/Gallery/{0}/{1}/{2}/{3}{4}.jpg", photo_row["Cate_FolderName"], photo_row["Sub_FolderName"], photo_row["Album_FolderName"], photo_row["FileNo"], filename);


                            //string album_folder_name = photo_row["Album_FolderName"].ToString();

                            //string sub_folderName = photo_row["Sub_FolderName"].ToString();

                            //string cate_folderName = photo_row["Cate_FolderName"].ToString();

                            //int albumID = Convert.ToInt32(photo_row["AlbumID"]);

                            //string chinese_album_name = AlbumID_AlbumName[albumID].ToString();

                            //string eng_album_name = photo_row["album_name"].ToString();

                            string album_folder_name = photo_row.Album_FolderName;

                            string sub_folderName = photo_row.Sub_FolderName;

                            string cate_folderName = photo_row.Cate_FolderName;

                            int albumID = Convert.ToInt32(photo_row.AlbumID);

                            string chinese_album_name = AlbumID_AlbumName[albumID].ToString().Replace("<br>", "");

                            string eng_album_name = photo_row.album_name.Replace("<br>", "");

                            rowID = Convert.ToInt32(photo_row.ID);


                            //string photo_url = string.Format(@"c:\test_album\{0}\{1}\{2}\{3}{4}.jpg", cate_folderName, sub_folderName, album_folder_name, photo_sortOrder_string, filename);
                            string photo_url = string.Format(@"c:\website\HKSPA_old_2\web\Gallery\{0}\{1}\{2}\{3}{4}.jpg", cate_folderName, sub_folderName, album_folder_name, photo_sortOrder_string, filename);


                            //get album name from web
                            //Dim album_name = GetAlbumName(photo_row("AlbumID"))

                            //context.Response.Write(String.Format("{0}<br />", album_name))


                            //album_result &= String.Format("album_name: {0}<br />", AlbumID_AlbumName.Values(photo_row["AlbumID"]));

                            //album_result &= String.Format("photo_url: <a href='{0}'>{0}</a><br />", photo_url)


                            Guid found_albumID = (from d in HKSPA_ms_db.AlbumNames
                                                  where d.AlbumName1 == chinese_album_name
                                                  && d.Lang == "zh-hk"
                                                  select d.AlbumID).FirstOrDefault();

                            //run if album not in DB
                            if (found_albumID.ToString() == "00000000-0000-0000-0000-000000000000")
                            {

                                string albumDate;
                                DateTime the_albumDate;

                                if (special_album_foldername.ContainsKey(album_folder_name))
                                {
                                    the_albumDate = special_album_foldername[album_folder_name];
                                }
                                else
                                {
                                    albumDate = album_folder_name.Substring(0, 8);
                                    the_albumDate = StringToDatetime(albumDate);
                                }

                                Guid new_AlbumID = Guid.NewGuid();

                                //<----------------------------Insert new album-------------------------->
                                Album new_album = new Album
                                {
                                    AlbumID = new_AlbumID,
                                    AlbumName = chinese_album_name,
                                    Description = chinese_album_name,
                                    PhotoCount = 0,
                                    Enabled = true,
                                    SortOrder = 0,
                                    AlbumDate = new DateTime(the_albumDate.Year, the_albumDate.Month, the_albumDate.Day),
                                    CreateDate = DateTime.Now,
                                    CreatedBy = "admin",
                                    UpdateDate = DateTime.Now,
                                    UpdatedBy = "admin"
                                };

                                // Add the new object to the Orders collection.
                                HKSPA_ms_db.Albums.InsertOnSubmit(new_album);

                                // Submit the change to the database. 
                                try
                                {
                                    HKSPA_ms_db.SubmitChanges();
                                }
                                catch (Exception e)
                                {
                                    txt_result.Text += string.Format("[New Album]Error of {0}. RowID: {1} \r\n", e.Message.ToString(), rowID);
                                    // Make some adjustments. 
                                    // ... 
                                    // Try again.
                                    HKSPA_ms_db.SubmitChanges();
                                }


                                //<----------------------------Insert new chinese album name -------------------------->
                                AlbumName new_chinese_album_name = new AlbumName
                                {
                                    AlbumID = new_AlbumID,
                                    AlbumName1 = chinese_album_name,
                                    Description = chinese_album_name,
                                    Lang = "zh-hk"
                                };

                                // Add the new object to the Orders collection.
                                HKSPA_ms_db.AlbumNames.InsertOnSubmit(new_chinese_album_name);

                                // Submit the change to the database. 
                                try
                                {
                                    HKSPA_ms_db.SubmitChanges();
                                }
                                catch (Exception e)
                                {
                                    txt_result.Text += string.Format("[Insert Chinese Album Name]Error of {0}. RowID: {1} \r\n", e.Message.ToString(), rowID);
                                    // Make some adjustments. 
                                    // ... 
                                    // Try again.
                                    HKSPA_ms_db.SubmitChanges();
                                }


                                //<----------------------------Insert new eng album name -------------------------->
                                AlbumName new_eng_album_name = new AlbumName
                                {
                                    AlbumID = new_AlbumID,
                                    AlbumName1 = eng_album_name,
                                    Description = eng_album_name,
                                    Lang = "en-us"
                                };

                                // Add the new object to the Orders collection.
                                HKSPA_ms_db.AlbumNames.InsertOnSubmit(new_eng_album_name);

                                // Submit the change to the database. 
                                try
                                {
                                    HKSPA_ms_db.SubmitChanges();
                                }
                                catch (Exception e)
                                {
                                    txt_result.Text += string.Format("[Insert Eng Album Name] Error of {0}. RowID: {1} \r\n", e.Message.ToString(), rowID);
                                    // Make some adjustments. 
                                    // ... 
                                    // Try again.
                                    HKSPA_ms_db.SubmitChanges();
                                }

                                sub_category_name = sub_category_name.Replace("!V", "–");

                                //<----------------------------Map AlbumID and Category ID -------------------------->
                                int found_category_ID = (from s in secondLevel_CategoryID_CategoryName
                                                         where s.Value.ToString().ToLower() == sub_category_name.ToLower()
                                                         select s.Key).First();

                                AlbumCategory new_AlbumCategory_Mapping = new AlbumCategory
                                {
                                    AlbumID = new_AlbumID,
                                    CategoryID = found_category_ID
                                };

                                // Add the new object to the Orders collection.
                                HKSPA_ms_db.AlbumCategories.InsertOnSubmit(new_AlbumCategory_Mapping);

                                // Submit the change to the database. 
                                try
                                {
                                    HKSPA_ms_db.SubmitChanges();
                                }
                                catch (Exception e)
                                {
                                    txt_result.Text += string.Format("[Map AlbumID and Category ID]Error of {0}. RowID: {1} \r\n", e.Message.ToString(), rowID);
                                    // Make some adjustments. 
                                    // ... 
                                    // Try again.
                                    HKSPA_ms_db.SubmitChanges();
                                }

                                //set album ID for copy photo
                                found_albumID = new_AlbumID;

                                ////create album folder
                                //string new_album_folder = string.Format(@"{0}\{1}",new_album_path, found_albumID);

                                //bool isExists = System.IO.Directory.Exists(new_album_folder);

                                //if (!isExists)
                                //{
                                //    System.IO.Directory.CreateDirectory(new_album_folder);
                                //}


                                ////create album Thumbnail folder
                                //string new_album_tb_folder = string.Format(@"{0}\tb", new_album_folder, found_albumID);

                                //isExists = System.IO.Directory.Exists(new_album_tb_folder);

                                //if (!isExists)
                                //{
                                //    System.IO.Directory.CreateDirectory(new_album_tb_folder);
                                //}
                            }

                            //create album folder
                            string new_album_folder = string.Format(@"{0}\{1}", new_album_path, found_albumID);

                            bool isExists = System.IO.Directory.Exists(new_album_folder);

                            if (!isExists)
                            {
                                System.IO.Directory.CreateDirectory(new_album_folder);
                            }


                            //create album Thumbnail folder
                            string new_album_tb_folder = string.Format(@"{0}\tb", new_album_folder, found_albumID);

                            isExists = System.IO.Directory.Exists(new_album_tb_folder);

                            if (!isExists)
                            {
                                System.IO.Directory.CreateDirectory(new_album_tb_folder);
                            }



                            int photo_found = (from d in HKSPA_ms_db.view_AlbumPhotoInfos
                                               where d.Lang == "en-us" && d.AlbumID == found_albumID
                                               && d.SortOrder == photo_sortOrder
                                               select d).Count();

                            if (photo_found == 0)
                            {
                                if (!skip_id.ContainsKey(rowID))
                                {
                                    if (File.Exists(photo_url))
                                    {
                                        CopyPhoto_FromOldAlbum_ToNewAlbum(photo_url, found_albumID, photo_sortOrder, author, camera_model);
                                        txt_result.Text += string.Format("Finish copy for {0} \r\n", rowID);

                                    }
                                    else
                                    {
                                        txt_result.Text += string.Format("File Not Exist! RowID: {0} \r\n", rowID);
                                    }
                                }


                            }
                        }

                    //}

                    //lbl_test.Text = album_result
                    //txt_result.Text += string.Format("Finish of {0} - {1} records \r\n", limit_from, limit_to);

                    //mysql_connection.Close();

                    //lbl_limitFrom.Text = (limit_from + range).ToString();

                    //lbl_limitTo.Text = (limit_to + range).ToString();


                    txt_result.Text += string.Format("End of copy photos. \r\n");

                }
            //}
            //catch (Exception ex)
            //{
                //txt_result.Text += string.Format("Error of {0}. RowID: {1} \r\n", ex.Message.ToString(), rowID);
                //MessageBox.Show(ex.Message.ToString());
            //}
        }

        public void CopyPhoto_FromOldAlbum_ToNewAlbum(string old_photo_url, Guid album_ID,int photo_sortOrder, string photo_author, string photo_camera_model)
        {
            Guid new_photoID = Guid.NewGuid();

            String new_photo_name = string.Format("{0}.jpg", new_photoID);

            //create album folder
            string copyTo_path = string.Format(@"{0}\{1}\{2}",new_album_path, album_ID, new_photo_name);


            string photo_tb_folder = string.Format(@"{0}\{1}\tb\{2}",new_album_path, album_ID, new_photo_name);

            //<----------------------------copy old photo to new album-------------------------->


            Dictionary<string,string[]> model_dictionary=new Dictionary<string,string[]>();
            model_dictionary.Add("Nikon D1H", new string[] { "NikonD1", "Nikon D100" });
            model_dictionary.Add("NikonD1", new string[] { "Nikon D1H" ,"Nikon D100" });
            model_dictionary.Add("D100", new string[] { "Nikon D1H", "NikonD1" });

            if (!File.Exists(old_photo_url))
            {
                if (model_dictionary.ContainsKey(photo_camera_model))
                {
                    string[] model_arr = model_dictionary[photo_camera_model];

                    string check_path="";
                    foreach (string model in model_arr)
                    {
                        check_path = old_photo_url.Replace(photo_camera_model, model);
                        if (File.Exists(check_path))
                        {
                            old_photo_url = check_path;
                        }
                    }
                }
            }

            System.IO.File.Copy(old_photo_url, copyTo_path);

            //using (FileStream fs = new FileStream(old_photo_url,  FileMode.Open, FileAccess.ReadWrite))
            //{
                using (Image Image = Image.FromFile(old_photo_url))
                {

                    //<----------------------------generate Thumbnail for photo -------------------------->
                    //Image Image = new Image.FromFile(old_photo_url);

                    //<----------------------------copy old photo to new album-------------------------->
                    System.IO.File.Copy(old_photo_url, photo_tb_folder);

                    Size ThumbnailSize = new Size(285, 285);
                    Image Image2 = ResizeImage(Image, ThumbnailSize, null, null, CropOption.FillTheArea);
                    SaveJPGWithCompressionSetting(Image2, photo_tb_folder, 100);
                    Image2.Dispose();
                    Image.Dispose();
                }
            //}

            //<----------------------------save new album photo -------------------------->
            AlbumPhoto new_AlbumPhoto = new AlbumPhoto
            {
                PhotoID = new_photoID,
                AlbumID = album_ID,
                PhotoName = new_photo_name,
                SortOrder=photo_sortOrder,
                CreateDate=DateTime.Now,
                CreatedBy="admin",
                UpdateDate = DateTime.Now,
                UpdatedBy="admin"
            };

            // Add the new object to the Orders collection.
            HKSPA_ms_db.AlbumPhotos.InsertOnSubmit(new_AlbumPhoto);

            // Submit the change to the database. 
            try
            {
                HKSPA_ms_db.SubmitChanges();
            }
            catch (Exception e)
            {
                txt_result.Text += string.Format("[Insert new photos] Error of {0}. RowID: {1} \r\n", e.Message.ToString(), rowID);
                // Make some adjustments. 
                // ... 
                // Try again.
                HKSPA_ms_db.SubmitChanges();
            }


            if (photo_sortOrder == 1)
            {
                
                SetAlbumCover(album_ID,new_photoID);

            }


            //<----------------------------save new album photo info -------------------------->
            insert_photo_info(old_photo_url, new_photoID, photo_author, photo_camera_model);

        }

        public void SetAlbumCover(Guid Album_ID,Guid photoID)
        {
            String cover_photo = string.Format("{0}.jpg", photoID.ToString());

            Album found_album = (from a in HKSPA_ms_db.Albums
                                where a.AlbumID == Album_ID
                                select a).First();

            found_album.PreviewUrl = cover_photo;

            // Submit the changes to the database. 
            try
            {
                HKSPA_ms_db.SubmitChanges();
            }
            catch (Exception e)
            {
                txt_result.Text += string.Format("[SetAlbumCover]Error of {0}. RowID: {1} \r\n", e.Message.ToString(), rowID);
                Console.WriteLine(e);
                // Provide for exceptions.
            }

        }

        public void insert_photo_info(string old_photo_url, Guid photoID, string photo_author,string input_camera_model)
        {

            String camera_model = "";
            String f_stop = "";
            String exposure_time = "";
            String iso_speed = "";
            String exposure_bias = "";
            String focal_length = "";
            String max_aperture = "";
            String meeting_mode = "";
            String subject_distance = "";
            String flash_mode = "";
            String shutter = "";
            String aperture = "";


            try
            {
            Goheer.EXIF.EXIFextractor image_info;

            //save image EXIF information
            image_info = CheckEXIF(old_photo_url);



            if (input_camera_model.Length == 0)
            {
                try
                {
                    camera_model = image_info["Equip Model"].ToString();
                }
                catch (Exception e)
                {

                    camera_model = "";
                }
            }
            else
            {
                camera_model = input_camera_model;
            }


            try
            {
                f_stop = image_info["F-Number"].ToString();
            }
            catch (Exception e)
            {
                f_stop = "";
            }

            try
            {
                exposure_time = image_info["Exposure Time"].ToString();
            }
            catch (Exception e)
            {
                exposure_time = "";
            }

            try
            {
                iso_speed = image_info["ISO Speed"].ToString();
            }
            catch (Exception e)
            {
                iso_speed = "";
            }

            try
            {
                exposure_bias = image_info["Exposure Bias"].ToString();
            }
            catch (Exception e)
            {
                exposure_bias = "";
            }

            try
            {
                focal_length = image_info["FocalLength"].ToString();
            }
            catch (Exception e)
            {
                focal_length = "";
            }

            try
            {
                max_aperture = image_info["MaxAperture"].ToString();
            }
            catch (Exception e)
            {
                max_aperture = "";
            }

            try
            {
                meeting_mode = image_info["Metering Mode"].ToString();
            }
            catch (Exception e)
            {
                meeting_mode = "";
            }

            try
            {
                subject_distance = image_info["Subject Loc"].ToString();
            }
            catch (Exception e)
            {
                subject_distance = "";
            }

            try
            {
                flash_mode = image_info["Flash"].ToString();
            }
            catch (Exception e)
            {
                flash_mode = "";
            }

            try
            {
                aperture = image_info["Aperture"].ToString();
            }
            catch (Exception e)
            {
                aperture = "";
            }

            try
            {
                shutter = image_info["Shutter Speed"].ToString();
            }
            catch (Exception e)
            {
                shutter = "";
            }

            }
            catch(Exception ex)
           {

               txt_result.Text += string.Format("[Insert Photo Info]Error of {0}. RowID: {1} \r\n", ex.Message.ToString(), rowID);



               //Bitmap bmp = new Bitmap(old_photo_url);

               //// Get the PropertyItems property from image.
               //PropertyItem[] propItems = bmp.PropertyItems;

               //foreach (PropertyItem propItem in propItems)
               //{
               //    if (propItem.Id.ToString("x") == "110")
               //    {
               //        if (input_camera_model.Length == 0)
               //        {
               //            camera_model = System.Text.Encoding.Unicode.GetString(propItem.Value);
               //        }
               //        else
               //        {
               //            camera_model = input_camera_model;
               //        }
               //    }


               //    if (propItem.Id.ToString("x") == "829D")
               //    {
               //        f_stop = propItem.GetType().ToString();
               //        f_stop = Convert.ToDouble(propItem.Value).ToString();

               //    }

               //    if (propItem.Id.ToString("x") == "829A")
               //    {

               //        exposure_time = Convert.ToInt32(propItem.Value).ToString();

               //    }

               //    if (propItem.Id.ToString("x") == "8827")
               //    {

               //        iso_speed = System.Text.Encoding.UTF8.GetString(propItem.Value);

               //    }

               //    if (propItem.Id.ToString("x") == "9204")
               //    {

               //        exposure_bias = System.Text.Encoding.UTF8.GetString(propItem.Value);

               //    }

               //    if (propItem.Id.ToString("x") == "920A")
               //    {

               //        focal_length = System.Text.Encoding.UTF8.GetString(propItem.Value);

               //    }

               //    if (propItem.Id.ToString("x") == "9205")
               //    {

               //        max_aperture = System.Text.Encoding.UTF8.GetString(propItem.Value);

               //    }


               //    if (propItem.Id.ToString("x") == "9207")
               //    {

               //        meeting_mode = System.Text.Encoding.UTF8.GetString(propItem.Value);

               //    }

               //    if (propItem.Id.ToString("x") == "A214")
               //    {

               //        subject_distance = System.Text.Encoding.UTF8.GetString(propItem.Value);

               //    }

               //    if (propItem.Id.ToString("x") == "9209")
               //    {

               //        flash_mode = System.Text.Encoding.UTF8.GetString(propItem.Value);

               //    }

               //    if (propItem.Id.ToString("x") == "9202")
               //    {

               //        aperture = System.Text.Encoding.UTF8.GetString(propItem.Value);

               //    }

               //    if (propItem.Id.ToString("x") == "9201")
               //    {

               //        shutter = System.Text.Encoding.UTF8.GetString(propItem.Value);

               //    }
               //}


          }
             


            

        

           

            




            


            AlbumPhoto_info photo_info = new AlbumPhoto_info
            {
                PhotoID = photoID,
                camera_model =  camera_model,
                f_stop = f_stop,
                exposure_time = exposure_time,
                iso_speed = iso_speed,
                exposure_bias = exposure_bias,
                focal_length = focal_length,
                max_aperture = max_aperture,
                meeting_mode =  meeting_mode,
                subject_distance = subject_distance,
                flash_mode = flash_mode,
                Author = photo_author,
                aperture =aperture,
                shutter =  shutter
            };

            // Add the new object to the Orders collection.
            HKSPA_ms_db.AlbumPhoto_infos.InsertOnSubmit(photo_info);

            // Submit the change to the database. 
            try
            {
                HKSPA_ms_db.SubmitChanges();
            }
            catch (Exception e)
            {
                txt_result.Text += string.Format("[insert_photo_info]Error of {0}. RowID: {1} \r\n", e.Message.ToString(), rowID);
                // Make some adjustments. 
                // ... 
                // Try again.
                HKSPA_ms_db.SubmitChanges();
            }


        }

        protected void tmr_copyData_Tick(object sender, System.EventArgs e)
        {
            getPhotoDetail(Convert.ToInt32(lbl_limitFrom.Text), Convert.ToInt32(lbl_limitTo.Text));

        }


        protected Goheer.EXIF.EXIFextractor CheckEXIF(String file_path)
        {

        /// "Exif IFD"
        /// "Gps IFD"
        /// "New Subfile Type"
        /// "Subfile Type"
        /// "Image Width"
        /// "Image Height"
        /// "Bits Per Sample"
        /// "Compression"
        /// "Photometric Interp"
        /// "Thresh Holding"
        /// "Cell Width"
        /// "Cell Height"
        /// "Fill Order"
        /// "Document Name"
        /// "Image Description"
        /// "Equip Make"
        /// "Equip Model"
        /// "Strip Offsets"
        /// "Orientation"
        /// "Samples PerPixel"
        /// "Rows Per Strip"
        /// "Strip Bytes Count"
        /// "Min Sample Value"
        /// "Max Sample Value"
        /// "X Resolution"
        /// "Y Resolution"
        /// "Planar Config"
        /// "Page Name"
        /// "X Position"
        /// "Y Position"
        /// "Free Offset"
        /// "Free Byte Counts"
        /// "Gray Response Unit"
        /// "Gray Response Curve"
        /// "T4 Option"
        /// "T6 Option"
        /// "Resolution Unit"
        /// "Page Number"
        /// "Transfer Funcition"
        /// "Software Used"
        /// "Date Time"
        /// "Artist"
        /// "Host Computer"
        /// "Predictor"
        /// "White Point"
        /// "Primary Chromaticities"
        /// "ColorMap"
        /// "Halftone Hints"
        /// "Tile Width"
        /// "Tile Length"
        /// "Tile Offset"
        /// "Tile ByteCounts"
        /// "InkSet"
        /// "Ink Names"
        /// "Number Of Inks"
        /// "Dot Range"
        /// "Target Printer"
        /// "Extra Samples"
        /// "Sample Format"
        /// "S Min Sample Value"
        /// "S Max Sample Value"
        /// "Transfer Range"
        /// "JPEG Proc"
        /// "JPEG InterFormat"
        /// "JPEG InterLength"
        /// "JPEG RestartInterval"
        /// "JPEG LosslessPredictors"
        /// "JPEG PointTransforms"
        /// "JPEG QTables"
        /// "JPEG DCTables"
        /// "JPEG ACTables"
        /// "YCbCr Coefficients"
        /// "YCbCr Subsampling"
        /// "YCbCr Positioning"
        /// "REF Black White"
        /// "ICC Profile"
        /// "Gamma"
        /// "ICC Profile Descriptor"
        /// "SRGB RenderingIntent"
        /// "Image Title"
        /// "Copyright"
        /// "Resolution X Unit"
        /// "Resolution Y Unit"
        /// "Resolution X LengthUnit"
        /// "Resolution Y LengthUnit"
        /// "Print Flags"
        /// "Print Flags Version"
        /// "Print Flags Crop"
        /// "Print Flags Bleed Width"
        /// "Print Flags Bleed Width Scale"
        /// "Halftone LPI"
        /// "Halftone LPIUnit"
        /// "Halftone Degree"
        /// "Halftone Shape"
        /// "Halftone Misc"
        /// "Halftone Screen"
        /// "JPEG Quality"
        /// "Grid Size"
        /// "Thumbnail Format"
        /// "Thumbnail Width"
        /// "Thumbnail Height"
        /// "Thumbnail ColorDepth"
        /// "Thumbnail Planes"
        /// "Thumbnail RawBytes"
        /// "Thumbnail Size"
        /// "Thumbnail CompressedSize"
        /// "Color Transfer Function"
        /// "Thumbnail Data"
        /// "Thumbnail ImageWidth"
        /// "Thumbnail ImageHeight"
        /// "Thumbnail BitsPerSample"
        /// "Thumbnail Compression"
        /// "Thumbnail PhotometricInterp"
        /// "Thumbnail ImageDescription"
        /// "Thumbnail EquipMake"
        /// "Thumbnail EquipModel"
        /// "Thumbnail StripOffsets"
        /// "Thumbnail Orientation"
        /// "Thumbnail SamplesPerPixel"
        /// "Thumbnail RowsPerStrip"
        /// "Thumbnail StripBytesCount"
        /// "Thumbnail ResolutionX"
        /// "Thumbnail ResolutionY"
        /// "Thumbnail PlanarConfig"
        /// "Thumbnail ResolutionUnit"
        /// "Thumbnail TransferFunction"
        /// "Thumbnail SoftwareUsed"
        /// "Thumbnail DateTime"
        /// "Thumbnail Artist"
        /// "Thumbnail WhitePoint"
        /// "Thumbnail PrimaryChromaticities"
        /// "Thumbnail YCbCrCoefficients"
        /// "Thumbnail YCbCrSubsampling"
        /// "Thumbnail YCbCrPositioning"
        /// "Thumbnail RefBlackWhite"
        /// "Thumbnail CopyRight"
        /// "Luminance Table"
        /// "Chrominance Table"
        /// "Frame Delay"
        /// "Loop Count"
        /// "Pixel Unit"
        /// "Pixel PerUnit X"
        /// "Pixel PerUnit Y"
        /// "Palette Histogram"
        /// "Exposure Time"
        /// "F-Number"
        /// "Exposure Prog"
        /// "Spectral Sense"
        /// "ISO Speed"
        /// "OECF"
        /// "Ver"
        /// "DTOrig"
        /// "DTDigitized"
        /// "CompConfig"
        /// "CompBPP"
        /// "Shutter Speed"
        /// "Aperture"
        /// "Brightness"
        /// "Exposure Bias"
        /// "MaxAperture"
        /// "SubjectDist"
        /// "Metering Mode"
        /// "LightSource"
        /// "Flash"
        /// "FocalLength"
        /// "Maker Note"
        /// "User Comment"
        /// "DTSubsec"
        /// "DTOrigSS"
        /// "DTDigSS"
        /// "FPXVer"
        /// "ColorSpace"
        /// "PixXDim"
        /// "PixYDim"
        /// "RelatedWav"
        /// "Interop"
        /// "FlashEnergy"
        /// "SpatialFR"
        /// "FocalXRes"
        /// "FocalYRes"
        /// "FocalResUnit"
        /// "Subject Loc"
        /// "Exposure Index"
        /// "Sensing Method"
        /// "FileSource"
        /// "SceneType"

        Bitmap bmp = new Bitmap(file_path);
        Goheer.EXIF.EXIFextractor er = new Goheer.EXIF.EXIFextractor( ref bmp, "\n");

        //MsgBox(er.Item("ISO Speed"))
        

        return er;
        }


        public enum ImageSize
        {
	        Thumbnail = 0,
	        Normal = 1
        }

        public enum CropOption
        {
	        FitInArea = 0,
	        FillTheArea = 1
        }

        public static Image ResizeImage(Image OriginalImage, Size NewSize, Image Watermark = null, Image Testmark = null, CropOption CropOption = CropOption.FitInArea)
        {
	        Size oSize = default(Size);
	        Size nSize = default(Size);
	        Bitmap bmp = default(Bitmap);
	        Graphics g = default(Graphics);
	        Rectangle rect = default(Rectangle);

	        oSize = OriginalImage.Size;
	        switch (CropOption) {
		        case CropOption.FitInArea:
			        nSize = ResizeInRatio(oSize, NewSize);
			        break;
		        case CropOption.FillTheArea:
			        nSize = NewSize;
			        break;
	        }

	        bmp = new Bitmap(nSize.Width, nSize.Height);
	        g = Graphics.FromImage(bmp);
	        g.CompositingQuality = CompositingQuality.HighQuality;
	        g.SmoothingMode = SmoothingMode.HighQuality;
	        g.InterpolationMode = InterpolationMode.High;

	        switch (CropOption) {
		        case CropOption.FitInArea:

			        break;
		        case CropOption.FillTheArea:
			        float ratio0 = NewSize.Width / NewSize.Height;
			        float ratio1 = OriginalImage.Width / OriginalImage.Height;
			        int W1 = 0;
			        int H1 = 0;
			        int X1 = 0;
			        int Y1 = 0;
			        if (ratio0 > ratio1) {
				        W1 = OriginalImage.Width;
				        H1 = NewSize.Height * OriginalImage.Width / NewSize.Width;
				        X1 = 0;
				        Y1 = 0;
				        // (OriginalImage.Height - H1) / 2
			        } else {
				        W1 = NewSize.Width * OriginalImage.Height / NewSize.Height;
				        H1 = OriginalImage.Height;
				        X1 = (OriginalImage.Width - W1) / 2;
				        Y1 = 0;
			        }
			        OriginalImage = Crop(OriginalImage, W1, H1, X1, Y1);
			        oSize = new Size(OriginalImage.Width, OriginalImage.Height);
			        break;
	        }

	        rect = new Rectangle(0, 0, nSize.Width, nSize.Height);
	        g.DrawImage(OriginalImage, rect, 0, 0, oSize.Width, oSize.Height, GraphicsUnit.Pixel);

	        if ((Watermark != null)) {
		        Rectangle watermarkRect = new Rectangle(0, nSize.Height - Watermark.Height, Watermark.Width, Watermark.Height);
		        g.DrawImage(Watermark, watermarkRect, 0, 0, Watermark.Width, Watermark.Height, GraphicsUnit.Pixel);
	        }

	        if ((Testmark != null)) {
		        Rectangle testmarkRect = new Rectangle(nSize.Width - Testmark.Width, 0, Testmark.Width, Testmark.Height);
		        g.DrawImage(Testmark, testmarkRect, 0, 0, Testmark.Width, Testmark.Height, GraphicsUnit.Pixel);
	        }

	        OriginalImage.Dispose();
	        return bmp;
        }

        public static Image Crop(Image img, int Width, int Height, int X, int Y)
        {
            Bitmap bmp = new Bitmap(Width, Height, img.PixelFormat);
            //bmp.SetResolution(img.HorizontalResolution, img.VerticalResolution)
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.DrawImage(img, new Rectangle(0, 0, Width, Height), X, Y, Width, Height, GraphicsUnit.Pixel);
            }
            return bmp;
        }

        public static Size ResizeInRatio(Size OriginalSize, Size TargetSize)
        {
            float ratio = 0;
            float ratio2 = 0;
            Size Size = default(Size);

            bool AllowBiggerSize = true;

            ratio = OriginalSize.Width / OriginalSize.Height;
            ratio2 = TargetSize.Width / TargetSize.Height;

            if ((OriginalSize.Width > TargetSize.Width | OriginalSize.Height > TargetSize.Height) | AllowBiggerSize)
            {
                if (ratio > ratio2)
                {
                    // resize to width
                    Size = new Size(TargetSize.Width, Convert.ToInt32(TargetSize.Width / OriginalSize.Width * OriginalSize.Height));
                }
                else
                {
                    // resize to Height
                    Size = new Size(Convert.ToInt32(TargetSize.Height / OriginalSize.Height * OriginalSize.Width), TargetSize.Height);
                }
            }
            else
            {
                Size = OriginalSize;
            }

            return Size;
        }

        public static void SaveJPGWithCompressionSetting(Image image, string szFileName, long lCompression)
        {
	        string ext = Path.GetExtension(szFileName).ToLower();
	        if (ext == ".jpg") {
		        EncoderParameters eps = new EncoderParameters(1);
		        eps.Param[0] = new EncoderParameter( System.Drawing.Imaging.Encoder.Quality, lCompression);
		        ImageCodecInfo ici = GetEncoderInfo("image/jpeg");
		        image.Save(szFileName, ici, eps);
	        } else {
		        image.Save(szFileName);
	        }
        }


        public static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            int j = 0;
            ImageCodecInfo[] encoders = null;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j <= encoders.Length; j++)
            {
                if (encoders[j].MimeType == mimeType)
                {
                    return encoders[j];
                }
            }
            return null;
        }

        public DateTime StringToDatetime(string input_Date)
        {
            CultureInfo enUS = new CultureInfo("en-US");
            DateTime dateValue;

            DateTime return_result = new DateTime(1970, 1, 1, 0, 0, 0);

            if (System.DateTime.TryParseExact(input_Date, "yyyyMdd", enUS, DateTimeStyles.None,  out dateValue))
            {
                Console.WriteLine("Converted '{0}' to {1} ({2}).", input_Date, dateValue, dateValue.Kind);

                return_result= dateValue;
                //lbl_test.Text = string.Format("Converted '{0}' to {1:yyyy-MM-dd} ({2}).", DateAndTime.DateString, dateValue(), dateValue().Kind);
            }
            else
            {
                Console.WriteLine("'{0}' is not in an acceptable format.", input_Date);
                //lbl_test.Text = string.Format("'{0}' is not in an acceptable format.", DateAndTime.DateString);
            }


            return return_result;

        }

        private void btn_mysql_record_Click(object sender, EventArgs e)
        {


            mysql_connection.Open();

            String selectString_count = "SELECT * FROM hkspa.album_view";

            MySqlCommand Count_Command = mysql_connection.CreateCommand();

            Count_Command.CommandText = selectString_count;

            MySqlDataReader count_Reader = null;

            count_Reader = Count_Command.ExecuteReader();

            DataTable count_view = new DataTable();

            count_view.Load(count_Reader);

            //find photo from album_view
            foreach (DataRow count_row in count_view.Rows)
            {
                old_album_photo_record insert_new_photo_record = new old_album_photo_record();

                insert_new_photo_record.main_categoryName = count_row["main_categoryName"].ToString();
                insert_new_photo_record.Cate_FolderName = count_row["Cate_FolderName"].ToString();
                insert_new_photo_record.sub_CategoryName = count_row["sub_CategoryName"].ToString();
                insert_new_photo_record.Sub_FolderName = count_row["Sub_FolderName"].ToString();
                insert_new_photo_record.album_name = count_row["album_name"].ToString();
                insert_new_photo_record.Album_FolderName = count_row["Album_FolderName"].ToString();
                insert_new_photo_record.AlbumID = Convert.ToInt32(count_row["AlbumID"].ToString());
                insert_new_photo_record.FileNo = count_row["FileNo"].ToString();
                insert_new_photo_record.fileName = count_row["fileName"].ToString();
                insert_new_photo_record.FileCam = count_row["FileCam"].ToString();
                insert_new_photo_record.PhotoW = count_row["PhotoW"].ToString();
                insert_new_photo_record.PhotoH = count_row["PhotoH"].ToString();

                // Add the new object to the Orders collection.
                HKSPA_ms_db.old_album_photo_records.InsertOnSubmit(insert_new_photo_record);


                // Submit the change to the database. 
                try
                {
                    HKSPA_ms_db.SubmitChanges();
                }
                catch (Exception ex)
                {
                    txt_result.Text += string.Format("Error of {0}. RowID: {1} \r\n", ex.Message.ToString(), rowID);
                    // Make some adjustments. 
                    // ... 
                    // Try again.
                    HKSPA_ms_db.SubmitChanges();
                }

                
            }

            //txt_result.Text += string.Format("{0} \r\n", "Finish insert from mysql.");


            mysql_connection.Close();

        


        }

        private void btn_copy_photo_Click(object sender, EventArgs e)
        {


            //root category ID of local DB(MSSQL)
            int[] local_MainCategory = new int[9];

            local_MainCategory[0] = 23;
            local_MainCategory[1] = 80;
            local_MainCategory[2] = 76;
            local_MainCategory[3] = 9;
            local_MainCategory[4] = 13;
            local_MainCategory[5] = 8;
            local_MainCategory[6] = 47;
            local_MainCategory[7] = 52;
            local_MainCategory[8] = 66;

            foreach (int categoryID in local_MainCategory)
            {
                IQueryable<view_Category> second_level_category = from d in HKSPA_ms_db.view_Categories
                                                                  where d.ParentID == categoryID && d.Lang == "en-us"
                                                                  select d;

                foreach (view_Category second_level_category_detail in second_level_category)
                {
                    secondLevel_CategoryID_CategoryName.Add(second_level_category_detail.CategoryID, second_level_category_detail.CategoryName);
                }


            }


            //mysql_connection.Open();

            //String selectString_count = "SELECT COUNT(*) as count_row FROM hkspa.spa_filedb";

            //MySqlCommand Count_Command = mysql_connection.CreateCommand();

            //Count_Command.CommandText = selectString_count;

            //MySqlDataReader count_Reader = null;

            //count_Reader = Count_Command.ExecuteReader();

            //DataTable count_view = new DataTable();

            //count_view.Load(count_Reader);

            ////find photo from album_view
            //foreach (DataRow count_row in count_view.Rows)
            //{
            //    total_count_row = Convert.ToInt32(count_row["count_row"].ToString());

            //}

            //mysql_connection.Close();

            int count_row = (from d in HKSPA_ms_db.old_album_photo_records
                             select d).Count();

            total_count_row = count_row;

            IQueryable<old_album> album_detail = from d in HKSPA_ms_db.old_albums
                                                 select d;

            foreach (old_album Album in album_detail)
            {
                AlbumID_AlbumName.Add(Album.old_Album_ID, Album.old_Album_Name);
            }


           // getPhotoDetail(Convert.ToInt32(lbl_limitFrom.Text), Convert.ToInt32(lbl_limitTo.Text));



            //tmr_copyData.Interval = 600000;
            //tmr_copyData.Tick += new EventHandler(tmr_copyData_Tick);
            //tmr_copyData.Start();

            txt_result.Text = "Start";

            Dictionary<string, DateTime> special_album_foldername = new Dictionary<string, DateTime>();
            Dictionary<int, int> skip_id = new Dictionary<int, int>();
            special_album_foldername.Add("2003athletics", new DateTime(2013, 12, 24, 0, 0, 0));
            special_album_foldername.Add("2003badminton", new DateTime(2013, 12, 24, 0, 0, 0));
            special_album_foldername.Add("2003boccia", new DateTime(2013, 12, 24, 0, 0, 0));
            special_album_foldername.Add("2003swimming", new DateTime(2013, 12, 24, 0, 0, 0));
            special_album_foldername.Add("2003table tennis", new DateTime(2013, 12, 24, 0, 0, 0));
            special_album_foldername.Add("200510", new DateTime(2005, 10, 01, 0, 0, 0));
            special_album_foldername.Add("200511", new DateTime(2005, 11, 01, 0, 0, 0));
            special_album_foldername.Add("200512_200601", new DateTime(2005, 12, 01, 0, 0, 0));
            special_album_foldername.Add("200601-05", new DateTime(2006, 01, 01, 0, 0, 0));
            special_album_foldername.Add("201209", new DateTime(2012, 09, 01, 0, 0, 0));
            special_album_foldername.Add("201210", new DateTime(2012, 10, 01, 0, 0, 0));
            special_album_foldername.Add("201211", new DateTime(2012, 11, 01, 0, 0, 0));
            special_album_foldername.Add("201212", new DateTime(2012, 12, 01, 0, 0, 0));


            skip_id.Add(2191, 2191); //2191	Ball Games	ball	Volleyball	volleyball	02-05 Nov 2003 -<br> 2nd Asian Schools Volleyball Championship (Boys) 2003	20031202-05	46	086	Fanling	Nikon D1H	419	640
            skip_id.Add(2192, 2192);//2192	Ball Games	ball	Volleyball	volleyball	02-05 Nov 2003 -<br> 2nd Asian Schools Volleyball Championship (Boys) 2003	20031202-05	46	087	Fanling	Nikon D1H	640	419
            skip_id.Add(2193, 2193);//2193	Ball Games	ball	Volleyball	volleyball	02-05 Nov 2003 -<br> 2nd Asian Schools Volleyball Championship (Boys) 2003	20031202-05	46	088	Fanling	Nikon D1H	640	419
            skip_id.Add(2194, 2194);//2194	Ball Games	ball	Volleyball	volleyball	02-05 Nov 2003 -<br> 2nd Asian Schools Volleyball Championship (Boys) 2003	20031202-05	46	089	Fanling	Nikon D1H	640	419

            skip_id.Add(2696, 2696);//Image Damage

            local_MainCategory[0] = 23;
            local_MainCategory[1] = 80;
            local_MainCategory[2] = 76;
            local_MainCategory[3] = 9;
            local_MainCategory[4] = 13;
            local_MainCategory[5] = 8;
            local_MainCategory[6] = 47;
            local_MainCategory[7] = 52;
            local_MainCategory[8] = 66;

            GetPhotoDetail longTest = new GetPhotoDetail(secondLevel_CategoryID_CategoryName, AlbumID_AlbumName, special_album_foldername, skip_id, local_MainCategory);
            System.Threading.Thread backgroundThread =
                new System.Threading.Thread(new System.Threading.ThreadStart(longTest.RunLoop));
            backgroundThread.Name = "BackgroundThread";
            backgroundThread.IsBackground = true;
            backgroundThread.Start();


        }


            


    }
}
