using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace TransMan
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        PMSEntities ent = new PMSEntities();
        JavaScriptSerializer jsSer = new JavaScriptSerializer();
        List<TranslationItem> transItems = new List<TranslationItem>();
        
        public EnumItem CurrentItem { get; set; }

        public EnumChild SelectedItem { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            CurrentItem = new EnumItem();

            dataGrid.ItemsSource = CurrentItem.EnumChilds;

            SelectedItem = null;
        }

        private void btInsert_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbKeyword.Text) || string.IsNullOrWhiteSpace(tbEnglish.Text) || string.IsNullOrWhiteSpace(tbGerman.Text))
            {
                MessageBox.Show("Keyword and one language must be filled!!!", "Warning");
                return;
            }

            insertData(tbKeyword.Text, tbEnglish.Text, tbGerman.Text);

            transItems.Add(new TranslationItem(tbKeyword.Text, tbEnglish.Text, tbGerman.Text, TranslationItem.OperationTypeEnum.Insert));//is called only from insert button, when I import from file I don't need to store the data, because they are already in the imported file

            showResult(tbKeyword.Text);
        }

        private void insertData(string keyword, string english, string german)
        {
            if (ent.Keywords.Where(x => x.identifier == tbKeyword.Text).Count() > 0)
            {
                MessageBox.Show("Keyword: " + tbKeyword.Text + " already is in DB!!!\nTry another one.");
                return;
            }

            var key = new Keyword() { isActive = 1, sortOrder = 1, usedIn = "Client", identifier = keyword };

            var eng = new Translation() { language = 4, translation1 = english };
            eng.Keyword = key;

            var ger = new Translation() { language = 2, translation1 = german };
            ger.Keyword = key;

            key.Translations.Add(eng);
            key.Translations.Add(ger);

            ent.Keywords.Add(key);

            ent.SaveChanges();

            /*
            insert into Keywords(identifier, sortOrder, isActive, usedIn)  VALUES('SWTK_WIZARD_STEP1_TECHCONTACT', 1, 1, 'Client');
            insert into Translations(translation, identifier, language) select 'Tech Contact', id, '4' from Keywords where identifier = 'SWTK_WIZARD_STEP1_TECHCONTACT';
            insert into Translations(translation, identifier, language) select 'Tech Kontakt', id, '2' from Keywords where identifier = 'SWTK_WIZARD_STEP1_TECHCONTACT';
            */
        }

        private void updateData(string oldkeyword, string keyword, string english, string german)
        {
            var keywords = ent.Keywords.Where(x => x.identifier == oldkeyword);

            if (keywords.Count() == 1)
            {
                //MessageBox.Show("Keyword: " + tbKeyword.Text + " already is in DB!!!\nTry another one.");

                var key = keywords.First();// { isActive = 1, sortOrder = 1, usedIn = "Client", identifier = keyword };
                key.identifier = keyword;

                key.Translations.Where(x => x.language == 4).First().translation1 = english;

                key.Translations.Where(x => x.language == 2).First().translation1 = german;

                ent.SaveChanges();
            }
            else
            {
                return;
            }
        }

        private void exportData(string fileName)
        {
            var json = jsSer.Serialize(transItems);

            File.WriteAllText(fileName, json);
        }

        private void btImport_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".tmj";
            dlg.Filter = "Translation manager json (*.tmj)|*.tmj|All files (*.*)|*.*";


            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {

                // Open document 
                string filename = dlg.FileName;
                importData(dlg.FileName);
            }

            MessageBox.Show("Import is done!", "Information");
        }

        private void importData(string fileName)
        {
            string json = File.ReadAllText(fileName);

            //string json = "[{\"Keyword\":\"KeywordValue\", \"English\":\"EnglishValue\", \"German\":\"GermanValue\"}, {\"Keyword\":\"KeywordValue\", \"English\":\"EnglishValue\", \"German\":\"GermanValue\"}]";

            List<TranslationItem> xxx = (List<TranslationItem>)jsSer.Deserialize(json, typeof(List<TranslationItem>));

            xxx.ForEach(x =>
            {
                if (x.Operation == TranslationItem.OperationTypeEnum.Insert)
                    insertData(x.Keyword, x.English, x.German);

                if (x.Operation == TranslationItem.OperationTypeEnum.Update)
                    updateData(x.Keyword, x.UpdatedItem.Keyword, x.UpdatedItem.English, x.UpdatedItem.German);
            });


        }

        private void btFind_Click(object sender, RoutedEventArgs e)
        {
            Keyword keyword = findItem(tbKeyword.Text);

            if (keyword != null)
            {
                tbEnglish.Text = keyword.Translations.Where(x => x.language == 4).First().translation1;

                tbGerman.Text = keyword.Translations.Where(x => x.language == 2).First().translation1;

                showResult(tbKeyword.Text);
            }
        }

        private Keyword findItem(string keyword)
        {
            var keyWords = ent.Keywords.Where(x => x.identifier == keyword);

            if (keyWords.Count() > 0)
            {
                if (keyWords.Count() > 1)
                {
                    MessageBox.Show("Keyword: " + tbKeyword.Text + " is in DB " + keyWords.Count().ToString() + " times!!!");
                    return null;
                }

                return keyWords.First();
            }

            MessageBox.Show("Keyword: " + tbKeyword.Text + " does not exist in DB!!!");

            return null;
        }

        private void showResult(string keyword)
        {
            tbResult.Text = "{{ '" + keyword + "' | translate }}\n" + "$translate.instant('" + keyword + "')\n";
        }

        private void btExport_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".tmj";
            dlg.Filter = "Translation manager json (*.tmj)|*.tmj|All files (*.*)|*.*";


            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {

                // Open document 
                string filename = dlg.FileName;
                exportData(dlg.FileName);
            }

            MessageBox.Show("Export is done!", "Information");
        }

        private void btUpdate_Click(object sender, RoutedEventArgs e)
        {
            Keyword keyword = findItem(tbKeyword.Text);

            if (keyword != null)
            {
                TranslationItem ti = new TranslationItem(tbKeyword.Text, keyword.Translations.Where(x => x.language == 4).First().translation1, keyword.Translations.Where(x => x.language == 2).First().translation1, TranslationItem.OperationTypeEnum.Update);

                UpdateDialog ud = new UpdateDialog();

                ud.TranslationItem = ti;

                var result = ud.ShowDialog();

                if (result == true)
                {
                    updateData(ti.Keyword, ti.UpdatedItem.Keyword, ti.UpdatedItem.English, ti.UpdatedItem.German);

                    transItems.Add(ti);

                    tbEnglish.Text = ti.UpdatedItem.English;
                    tbKeyword.Text = ti.UpdatedItem.Keyword;
                    tbGerman.Text = ti.UpdatedItem.German;

                    showResult(tbKeyword.Text);
                }
            }
        }

        public void UpdateDataGridMethod(Keyword keyword)
        {

        }

        public delegate void UpdateDataGrid(Keyword message);

        private void btEnums_Click(object sender, RoutedEventArgs e)
        {
            Keyword keyword = findItem(tbKeyword.Text);

            UpdateDataGrid udg = UpdateDataGridMethod;

            if (keyword != null)
            {
                //dataGrid.ItemsSource = null;

                CurrentItem.EnumChilds.Add(new EnumChild() { LocKey = keyword.identifier, SortOrder = CurrentItem.EnumChilds.Count(), IsDefault = CurrentItem.EnumChilds.Count() == 0 });
//                dataGrid.ItemsSource = CurrentItem.EnumChilds;
                tabControl.SelectedIndex = 1;
            }
            else
            {
                MessageBox.Show("First insert the item to DB!!!", "Warning");
            }
        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedItem != null)//if not null then set values to old selected item
            {
                SelectedItem.LocKey = tbKey.Text;
                SelectedItem.SortOrder = Convert.ToInt32(tbSort.Text);
                SelectedItem.Value = tbValue.Text;
                SelectedItem.IsDefault = (bool)chbDefault.IsChecked;
            }
            
            SelectedItem = (EnumChild)dataGrid.SelectedItem;//set new selected item            
            
            if (SelectedItem == null) return;

            dataGrid.Items.Refresh();

            tbKey.Text = SelectedItem.LocKey;
            tbSort.Text = SelectedItem.SortOrder.ToString();
            tbValue.Text = SelectedItem.Value;
            chbDefault.IsChecked = SelectedItem.IsDefault;
        }

        private void btSaveEnum_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbType.Text))
            {
                MessageBox.Show("Type must be filled!!!", "Warning");
                return;
            }

            if (CurrentItem.EnumChilds.Count() == 0)
            {
                MessageBox.Show("Enumeration must have connected some keywords!!!", "Warning");
                return;
            }
        }

        private void tbSort_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !isTextAllowed(e.Text);
        }

        private static bool isTextAllowed(string text)
        {
            Regex regex = new Regex("[^0-9.]"); //regex that matches disallowed text
            return !regex.IsMatch(text);
        }

        private void tbValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SelectedItem != null)
            {
                SelectedItem.Value = tbValue.Text;
                dataGrid.Items.Refresh();
            }
        }

        private void tbSort_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SelectedItem != null)
            {
                SelectedItem.SortOrder = Convert.ToInt32(tbSort.Text);
                dataGrid.Items.Refresh();
            }
        }

        private void chbDefault_Checked(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(chbDefault.IsChecked.ToString());
        }

        private void chbDefault_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(chbDefault.IsChecked.ToString());
        }       
    }
}
