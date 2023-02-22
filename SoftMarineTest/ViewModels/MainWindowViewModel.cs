using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using SoftMarineTest.Models;
using SoftMarineTest.MVVM;
using SoftMarineTest.Views;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SoftMarineTest.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly string _connectionString = @"Data Source=LAPTOP-M2DU7BFK;Initial Catalog=SoftMarineTestBD;Integrated Security=True";
        private readonly string _downArrowPath = "/Assets/downarrow.png";
        private readonly string _upArrowPath = "/Assets/uparrow.png";

        public MainWindowViewModel()
        {
            DBInitializing();
        }

        private Section _sectionSelectedItem = new Section();
        public Section SectionSelectedItem
        {
            get => _sectionSelectedItem;
            set => this.SetProperty(ref this._sectionSelectedItem, value);
        }

        private ObservableCollection<Section> _sectionsList = new ObservableCollection<Section>();
        public ObservableCollection<Section> SectionsList
        {
            get => _sectionsList;
            set => this.SetProperty(ref this._sectionsList, value);
        }

        private ObservableCollection<Section> _visibleSectionsList = new ObservableCollection<Section>();
        public ObservableCollection<Section> VisibleSectionsList
        {
            get => _visibleSectionsList;
            set => this.SetProperty(ref this._visibleSectionsList, value);
        }


        private ICommand _addSectionCommand;
        public ICommand AddSectionCommand => _addSectionCommand ?? (_addSectionCommand = new DelegateCommand(() =>
        {
            AddSectionWindow addSectionWindow = new AddSectionWindow();
            foreach (Section s in _sectionsList)
            {
                (addSectionWindow.DataContext as AddSectionWindowViewModel).AllSections.Add(s);
            }
            (addSectionWindow.DataContext as AddSectionWindowViewModel).InizializingComboBox();
            (addSectionWindow.DataContext as AddSectionWindowViewModel).AddNewSection += (sender, args) =>
            {
                AddNewSection((addSectionWindow.DataContext as AddSectionWindowViewModel).SelectedParentSection.sectionCode, (addSectionWindow.DataContext as AddSectionWindowViewModel).NewSectionName);
            };
            addSectionWindow.ShowDialog();

        }));

        private ICommand _editSectionCommand;
        public ICommand EditSectionCommand => _editSectionCommand ?? (_editSectionCommand = new DelegateCommand(() =>
        {


        }));

        private ICommand _searchSectionCommand;
        public ICommand SearchSectionCommand => _searchSectionCommand ?? (_searchSectionCommand = new DelegateCommand(() =>
        {


        }));

        private ICommand _openSectionCommand;
        public ICommand OpenSectionCommand => _openSectionCommand ?? (_openSectionCommand = new DelegateCommand(() =>
        {
            foreach (Section section in _sectionsList)
            {
                if (section.parentSectionCode == SectionSelectedItem.sectionCode)
                {
                    if (!_visibleSectionsList.Contains(section))
                    {
                        _visibleSectionsList.Add(section);                       
                    }
                    else
                    {
                        RemoveChildrenSection(section);
                    }
                }
                else
                {
                    if (section == SectionSelectedItem)
                    {
                        if (section.arrowPath == _upArrowPath) section.arrowPath = _downArrowPath;
                        else section.arrowPath = _upArrowPath;
                    }
                }
                    
            }

            SortCollection();

        }));

        private void AddNewSection(string parentCode, string name)
        {
            Section newSection;
            if (parentCode != "0")
            {
                var parent = _sectionsList.Single(x => x.sectionCode == parentCode);
                int countChildren = 0;
                foreach (Section s in _sectionsList)
                {
                    if (s.parentSectionCode == parentCode) countChildren++;
                }
                newSection = new Section(parent.sectionCode + "." + (countChildren + 1).ToString(), parent.sectionCode, AddIndentToName(parentCode, name));
                _sectionsList.Add(newSection); 

                if (parent.hasChildren == false)
                {
                    _sectionsList.Single(x => x.sectionCode == parentCode).hasChildren = true;
                    _visibleSectionsList.Add(newSection);
                    parent.arrowPath = _upArrowPath;              
                }
                else
                {
                    if (parent.arrowPath == _upArrowPath) _visibleSectionsList.Add(newSection);
                }

                SortCollection();
            }
            else
            {
                var mainSections = _sectionsList.Where(x => x.parentSectionCode == "0");
                mainSections.OrderBy(x => x.sectionCode);
                int newSectionNum = int.Parse(mainSections.Last().sectionCode) + 1;
                newSection = new Section(newSectionNum.ToString(), "0", name);
                _sectionsList.Add(newSection);
                _visibleSectionsList.Add(newSection);
            }

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand();
                string sqlExpression = String.Format("INSERT INTO Sections (id, parent_id, name) VALUES ('{0}', '{1}','{2}')", newSection.sectionCode, parentCode, name);
                command.CommandText = sqlExpression;
                command.Connection = connection;
                int number = command.ExecuteNonQuery();
            }
        }

        private void RemoveChildrenSection(Section section)
        {
            foreach (Section children in _sectionsList)
            {
                if (children.parentSectionCode == section.sectionCode)
                {
                    if (_visibleSectionsList.Contains(children))
                    {
                        _visibleSectionsList.Remove(children);
                    }
                    if (children.hasChildren)
                    {
                        RemoveChildrenSection(children);
                    }
                }
            }
            section.arrowPath = _downArrowPath;
            _visibleSectionsList.Remove(section);
        }


        private void SortCollection()
        {
            List<Section> sortedVisibleList = _visibleSectionsList.OrderBy(x => x.sectionCode).ToList();
            _visibleSectionsList.Clear();
            foreach (Section sortedSection in sortedVisibleList)
            {
                _visibleSectionsList.Add(sortedSection);
            }

            List<Section> sortedList = _sectionsList.OrderBy(x => x.sectionCode).ToList();
            _sectionsList.Clear();
            foreach (Section sortedSection in sortedList)
            {
                _sectionsList.Add(sortedSection);
            }    
        }

        private string AddIndentToName(string parentId, string name)
        {
            int count = parentId.ToString().Count(c => c == '.');
            string space = new string(' ', (count + 1) * 2);
            return space + name.ToString();
        }

        private void DBInitializing()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand();
                command.CommandText = "SELECT * FROM Sections";
                command.Connection = connection;
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        object id = reader.GetValue(0);
                        object prntId = reader.GetValue(1);
                        object name = reader.GetValue(2);

                        string parentId = prntId.ToString();
                        string sectionName = name.ToString();
                        if (parentId != "")
                        {
                            sectionName = AddIndentToName(parentId, sectionName);
                        }
                        else
                        {
                            parentId = "0";
                        }

                        Section section = new Section(id.ToString(), parentId, sectionName);
                        _sectionsList.Add(section);
                    }
                }

                reader.Close();

                foreach (Section section in _sectionsList)
                {
                    if (section.parentSectionCode == "0") _visibleSectionsList.Add(section);
                    foreach (Section otherSection in _sectionsList)
                    {
                        if (otherSection.parentSectionCode == section.sectionCode)
                        {
                            section.hasChildren = true;
                        }
                    }
                }
            }
        }
    }
}
