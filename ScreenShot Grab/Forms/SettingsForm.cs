﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace ScreenShot_Grab
{
    public partial class SettingsForm : Form
    {

        public class ComboboxItem
        {
            public string Text { get; set; }
            public string Value { get; set; }

            public override string ToString()
            {
                return Text;
            }
            public ComboboxItem(string text, string value)
            {
                Text = text;
                Value = value;
            }
        }

        public MainForm form1;

        public SettingsForm(MainForm parent)
        {
            form1 = parent;
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            cutborder.Checked = Properties.Settings.Default.cutborder;
            path.Text = form1.spath;
            svurl.Text = Properties.Settings.Default.svurl;
            useragent.Text = Properties.Settings.Default.agent;
            ignoressl.Checked = Properties.Settings.Default.ignoressl;
            opendir.Checked = Properties.Settings.Default.opendir;
            openlink.Checked = Properties.Settings.Default.openlink;
            copylink.Checked = Properties.Settings.Default.copylink;
            tabControl1.SelectedIndex = Properties.Settings.Default.service;
            editor.Text = Properties.Settings.Default.editor;
            sysedit.Checked = Properties.Settings.Default.sysedit;
            editor.Enabled = !sysedit.Checked;
            editselect.Enabled = editor.Enabled;
            //imgur.Text = Properties.Settings.Default.imgur;
            RegImgur();
            format.SelectedIndex = Properties.Settings.Default.format;
            quality.Enabled = (format.SelectedIndex == 1 ? true : false);
            quality.Value = Properties.Settings.Default.quality;
            adv.Enabled = cutborder.Checked;
            //language.SelectedIndex = Properties.Settings.Default.language;
            autoshow.Checked = Properties.Settings.Default.autoshow;
            logfile.Text = Properties.Settings.Default.logfile;
            logfilecheck.Checked = Properties.Settings.Default.writelog;
            logfilebrowse.Enabled = logfilecheck.Checked;
            logfile.Enabled = logfilecheck.Checked;
            clipboard.Checked = Properties.Settings.Default.clipboard;

            language.Items.Clear();
            foreach (CultureInfo item in GetSupportedCulture()) {
                var lc = item.TwoLetterISOLanguageName;
                var citem = new ComboboxItem(item.NativeName, lc);
                //Debug.WriteLine(item.NativeName);
                if (item.Name == CultureInfo.InvariantCulture.Name) {
                    lc = "ru";
                    citem = new ComboboxItem("Русский", lc);
                }
                language.Items.Add(citem);
                if (Properties.Settings.Default.language == lc) {
                    language.SelectedItem = citem;
                }
            }
            /*
            if (Screen.PrimaryScreen.WorkingArea.Height<Height) {
                Height = Screen.PrimaryScreen.WorkingArea.Height;
            }*/
            //AutoScrollPosition = new Point(0, 0);
            //VerticalScroll.Value = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.cutborder = cutborder.Checked;
            var spath = path.Text;
            if (!Directory.Exists(spath)) {
                MessageBox.Show(form1.LocM.GetString("path_nf"), form1.LocM.GetString("error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            } else {
                form1.spath = spath;
                if (spath == Application.StartupPath + @"\src\") { spath = ""; }
                Properties.Settings.Default.scrpath = spath;
                Properties.Settings.Default.svurl = svurl.Text;
                Properties.Settings.Default.agent = useragent.Text;
                Properties.Settings.Default.ignoressl = ignoressl.Checked;
                Properties.Settings.Default.opendir = opendir.Checked;
                Properties.Settings.Default.openlink = openlink.Checked;
                Properties.Settings.Default.copylink = copylink.Checked;
                Properties.Settings.Default.service = tabControl1.SelectedIndex;
                //Properties.Settings.Default.imgur = imgur.Text;
                Properties.Settings.Default.editor = editor.Text;
                Properties.Settings.Default.sysedit = sysedit.Checked;
                Properties.Settings.Default.format = format.SelectedIndex;
                Properties.Settings.Default.quality = Convert.ToInt32(quality.Value);
                Properties.Settings.Default.autoshow = autoshow.Checked;
                if (logfilecheck.Checked!=Properties.Settings.Default.writelog) {
                    form1.EnableLog(!logfilecheck.Checked);
                }
                Properties.Settings.Default.writelog = logfilecheck.Checked;
                Properties.Settings.Default.logfile = logfile.Text;
                Properties.Settings.Default.clipboard = clipboard.Checked;
                Properties.Settings.Default.Save();
                if (Properties.Settings.Default.ignoressl) {
                    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                } else {
                    ServicePointManager.ServerCertificateValidationCallback = null;
                }
                Close();
            }
        }

        internal void RegImgur()
        {
            if (Properties.Settings.Default.account != "") {
                imgurb.Text = form1.LocM.GetString("unlink_acc");
                status.Text = form1.LocM.GetString("link_acc_status") + " " + Properties.Settings.Default.account;
            } else {
                imgurb.Text = form1.LocM.GetString("link_acc");
                status.Text = form1.LocM.GetString("noacc");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = path.Text;
            var result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK) {
                path.Text = folderBrowserDialog1.SelectedPath+@"\";
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            adv.Enabled = cutborder.Checked;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://api.imgur.com/oauth2/addclient/");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (editor.Text == "mspaint.exe") {
                openFileDialog1.InitialDirectory = Environment.GetEnvironmentVariable("ProgramFiles");
                openFileDialog1.FileName = "";
            } else {
                openFileDialog1.InitialDirectory = editor.Text;
                openFileDialog1.FileName = Path.GetFileName(editor.Text);
            }
            openFileDialog1.ShowDialog();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            editor.Text = openFileDialog1.FileName;
        }

        private void sysedit_CheckedChanged(object sender, EventArgs e)
        {
            editor.Enabled = !sysedit.Checked;
            editselect.Enabled = editor.Enabled;
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.account != "") {
                Properties.Settings.Default.account = "";
                Properties.Settings.Default.access_token = "";
                Properties.Settings.Default.refresh_token = "";
                Properties.Settings.Default.Save();
                RegImgur();
            } else {
                imgurb.Enabled = false;
                System.Diagnostics.Process.Start("https://api.imgur.com/oauth2/authorize?client_id=" + form1.clientid + "&response_type=pin&state=APPLICATION_STATE");
                Thread.Sleep(1000);
                var form = new PinForm(this);
                form.ShowDialog();
                imgurb.Enabled = true;
            }
        }

        private void format_SelectedIndexChanged(object sender, EventArgs e)
        {
            quality.Enabled = (format.SelectedIndex == 1 ? true : false);
        }

        private void quality_ValueChanged(object sender, EventArgs e)
        {
            quality.Value = Convert.ToInt32(quality.Value);
        }

        private void adv_Click(object sender, EventArgs e)
        {
            var form = new CropForm();
            form.ShowDialog();
        }

        private void reset_Click(object sender, EventArgs e)
        {
            form1.spath = Application.StartupPath + @"\src\";
            var curlang = ((ComboboxItem)language.SelectedItem).Value;
            Properties.Settings.Default.Reset();
            Properties.Settings.Default.language = curlang;
            Form2_Load(sender, e);
            //UpdateLang(language.SelectedIndex);
        }

        private void language_SelectedIndexChanged(object sender, EventArgs e)
        {
            var lang = ((ComboboxItem)language.SelectedItem).Value;
            if (Properties.Settings.Default.language == lang) return;
            UpdateLang(lang);
        }

        private void UpdateLang(string lang)
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang);
            ChangeLanguage(lang);
            Properties.Settings.Default.language = lang;
            Properties.Settings.Default.Save();
            RegImgur();/*
            if (Screen.PrimaryScreen.WorkingArea.Height < Height) {
                Height = Screen.PrimaryScreen.WorkingArea.Height;
            }*/
            form1.OnLangChange();
        }

        // (c) http://stackoverflow.com/questions/21067507/change-language-at-runtime-in-c-sharp-winform
        private void ChangeLanguage(string lang)
        {
            foreach (Form frm in Application.OpenForms) {
                localizeForm(frm);
            }
        }

        private void localizeForm(Form frm)
        {
            var manager = new ComponentResourceManager(frm.GetType());
            manager.ApplyResources(frm, "$this");
            applyResources(manager, frm.Controls);
        }

        private void applyResources(ComponentResourceManager manager, Control.ControlCollection ctls)
        {
            foreach (Control ctl in ctls) {
                manager.ApplyResources(ctl, ctl.Name);
                Debug.WriteLine(ctl.Name);
                applyResources(manager, ctl.Controls);
            }
        }

        // (c) http://stackoverflow.com/questions/553244/programmatic-way-to-get-all-the-available-languages-in-satellite-assemblies
        private IList<CultureInfo> GetSupportedCulture()
        {
            //Get all culture 
            CultureInfo[] culture = CultureInfo.GetCultures(CultureTypes.AllCultures);

            //Find the location where application installed.
            string exeLocation = Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path));

            //Return all culture for which satellite folder found with culture code.
            //return culture.Where(cultureInfo => Directory.Exists(Path.Combine(exeLocation, cultureInfo.Name)));
            IList<CultureInfo> cultures = new List<CultureInfo>();
            foreach(var cultureInfo in culture) {
                if (Directory.Exists(Path.Combine(exeLocation, cultureInfo.Name))) {
                    cultures.Add(cultureInfo);
                }
            }
            return cultures;
        }

        private void ignoressl_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.ignoressl = ignoressl.Checked;
            Properties.Settings.Default.Save();
            if (Properties.Settings.Default.ignoressl) {
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            } else {
                ServicePointManager.ServerCertificateValidationCallback = null;
            }
        }

        private void logfilebrowse_Click(object sender, EventArgs e)
        {
            saveFileDialog1.InitialDirectory = logfile.Text;
            saveFileDialog1.ShowDialog();
        }

        private void logfilecheck_CheckedChanged(object sender, EventArgs e)
        {
            logfile.Enabled = logfilecheck.Checked;
            logfilebrowse.Enabled = logfilecheck.Checked;
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            var name = saveFileDialog1.FileName;
            logfile.Text = name;
        }
    }
}
