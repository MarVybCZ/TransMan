using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TransMan.Controls
{
    /// <summary>
    /// Interaction logic for SQLExport.xaml
    /// </summary>
    public partial class SQLExport : UserControl
    {
        PMSEntities ent = new PMSEntities();

        public SQLExport()
        {
            InitializeComponent();
        }

        private void btSQLExport_Click(object sender, RoutedEventArgs e)
        {
            //var swtk = ent.Keywords.Where(x => x.identifier.Contains("SWTK") || x.identifier.Contains("OKCANCEL"));

            if (string.IsNullOrWhiteSpace(tbSQLExport.Text))
            {
                MessageBox.Show("Search text must be filled!!!", "Warning");
                return;
            }

            // Create OpenFileDialog 
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".tmj";
            dlg.Filter = "SQL script file (*.sql)|*.sql|All files (*.*)|*.*";


            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();
            
            // Get the selected file name and display in a TextBox 
            if (result != true) return;

            var swtk = ent.Keywords.Where(x => x.identifier.Contains(tbSQLExport.Text));

            StringBuilder sb = new StringBuilder();

            foreach (Keyword kw in swtk)
            {
                sb.AppendLine(string.Format("IF (select COUNT(id) from Keywords where identifier like '{0}') = 0", kw.identifier));
                sb.AppendLine("BEGIN");
                /*
                    insert into Keywords (identifier,sortOrder,isActive,usedIn) VALUES ('SWTK_LABEL_SUPERVISOR',1,1,'Client');
                    insert into Translations (translation, identifier, language) select 'Supervisor', id, '4' from Keywords where identifier = 'SWTK_LABEL_SUPERVISOR';
                    insert into Translations (translation, identifier, language) select 'Supervisor', id, '2' from Keywords where identifier = 'SWTK_LABEL_SUPERVISOR';
                 */
                sb.AppendLine(string.Format("insert into Keywords (identifier,sortOrder,isActive,usedIn) VALUES ('{0}',1,1,'Client');", kw.identifier));
                sb.AppendLine(string.Format("insert into Translations (translation, identifier, language) select '{0}', id, '4' from Keywords where identifier = '{1}';", kw.Translations.Where(x => x.language == 4).FirstOrDefault().translation1, kw.identifier));
                sb.AppendLine(string.Format("insert into Translations (translation, identifier, language) select '{0}', id, '2' from Keywords where identifier = '{1}';", kw.Translations.Where(x => x.language == 2).FirstOrDefault().translation1, kw.identifier));
                sb.AppendLine("END");
                sb.AppendLine();
            }
            
            File.WriteAllText(dlg.FileName, sb.ToString());

            MessageBox.Show("SQL export is done!!!", "Information");
        }
    }
}
