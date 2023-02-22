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
    class AddSectionWindowViewModel : BindableBase
    {
        public event EventHandler CloseWindow;
        public event EventHandler AddNewSection;


        private ObservableCollection<Section> _allSections = new ObservableCollection<Section>();
        public ObservableCollection<Section> AllSections
        {
            get => _allSections;
            set => this.SetProperty(ref this._allSections, value);
        }

        private ObservableCollection<Section> _comboBoxItems = new ObservableCollection<Section>();
        public ObservableCollection<Section> ComboBoxItems
        {
            get => _comboBoxItems;
            set => this.SetProperty(ref this._comboBoxItems, value);
        }

        private Section _selectedParentSection; 
        public Section SelectedParentSection
        {
            get => _selectedParentSection;
            set => this.SetProperty(ref this._selectedParentSection, value);
        }

        private string _newSectionName = "";
        public string NewSectionName
        {
            get => _newSectionName;
            set => this.SetProperty(ref this._newSectionName, value);
        }

        private ICommand _cancelCommand;
        public ICommand CancelCommand => _cancelCommand ?? (_cancelCommand = new DelegateCommand(() =>
        {
            this.CloseWindow(this, EventArgs.Empty);
        }));

        private ICommand _addNewSectionCommand;
        public ICommand AddNewSectionCommand => _addNewSectionCommand ?? (_addNewSectionCommand = new DelegateCommand(() =>
        {
            this.AddNewSection(this, EventArgs.Empty);
            this.CloseWindow(this, EventArgs.Empty);
        }));

        public void InizializingComboBox()
        {
            foreach (Section section in _allSections)
            {
                string newName = section.sectionCode + " " + section.sectionName.TrimStart(' ');
                _comboBoxItems.Add(new Section(section.sectionCode, section.parentSectionCode, newName));
            }
            
            _comboBoxItems.Insert(0, new Section("0", "0", "(Нет)"));
        }
    }


}
