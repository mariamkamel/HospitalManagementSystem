﻿using HospitalManagementSystem.Models;
using System.Collections.ObjectModel;
using System;
using System.Windows.Input;
using System.Collections.Generic;
using HospitalManagementSystem.Services;
using System.Linq;
using System.Windows;

namespace HospitalManagementSystem.ViewModels
{
    public class EmployeesViewModel : BaseViewModel
    {
        /// <summary>
        /// Items Properties
        /// </summary>
        public ObservableCollection<EmployeeCardViewModel> Employees { get; set; }
        public ObservableCollection<EmployeeCardViewModel> FilteredEmployees { get; set; }

        /// <summary>
        /// Search Bar Properties
        /// </summary>
        public String SearchQuery { get; set; }
        
        /// <summary>
        /// Add Dialog Properites
        /// </summary>
        public String EmployeeNameTextBox { get; set;  }
        public String EmployeeAddressTextBox { get; set; }
        public String EmployeeSalaryTextBox { get; set; }
        public ComboBoxPairs EmployeeDepartment { get; set; }
        public DateTime EmployeeDatePicker { get; set; }
        public String EmpoloyeeRoleComboBox { get; set; }
        public String doc;
        public bool isHeadCheck { get; set; }
        public  String EmployeeRole
        {
            get => doc;
            set
            {
                if (value == "Doctor")
                    isHead = Visibility.Visible;
                else
                    isHead = Visibility.Collapsed;

                doc = value;
            }
        }

        public List<ComboBoxPairs> ComboBoxItems;
        public Visibility isHead { get; set; }

        public ICommand SearchAction { get; set; }

        public EmployeesViewModel()
        {
            EmployeeDatePicker = DateTime.Today;
            ComboBoxItems = new List<ComboBoxPairs>();
            SearchAction = new RelayCommand(Search);
            isHead = Visibility.Collapsed;
           
            foreach (Department department in Hospital.Departments.Values)
            {
                ComboBoxItems.Add(new ComboBoxPairs(department.ID, department.Name));
            }

            Employees = new ObservableCollection<EmployeeCardViewModel>();
            foreach (Employee employee in Hospital.Employees.Values)
            {
                Employees.Add(
                    new EmployeeCardViewModel
                    {
                        ID = employee.ID,
                        Name = employee.Name,
                        Role = (employee.GetType() == typeof(Doctor))? "Doctor" : "Nurse",
                        Department = employee.Department.Name,
                        Salary = employee.Salary.ToString() + '$'
                    }
                );
            }
            FilteredEmployees = new ObservableCollection<EmployeeCardViewModel>(Employees);
        }

        private void Search()
        {
            if (String.IsNullOrEmpty(SearchQuery))
            {
                FilteredEmployees = new ObservableCollection<EmployeeCardViewModel>(Employees);
                return;
            }

            FilteredEmployees = new ObservableCollection<EmployeeCardViewModel>(
                Employees.Where(employee => employee.Name.ToLower().Contains(SearchQuery))
            );
        }

        public void addEmployee()
        {
            if (EmployeeRole == "Doctor") {
                if(isHeadCheck)
                {
                    foreach(Employee employee in Hospital.Employees.Values)
                    {
                        if (employee.GetType() == typeof(Doctor))
                        {
                            if (Hospital.Departments[Hospital.Employees[employee.ID].Department.ID].Doctors[employee.ID].IsHead)
                            {
                                Hospital.Departments[Hospital.Employees[employee.ID].Department.ID].Doctors[employee.ID].IsHead = false;
                                break;
                            }
                        }
                    }
                }
                Doctor newDoctor = new Doctor
                {
                    
                    Name = EmployeeNameTextBox,
                    Salary = Double.Parse(EmployeeSalaryTextBox),
                    Department = Hospital.Departments[EmployeeDepartment.Key],
                    Address = EmployeeAddressTextBox,
                    IsHead = isHeadCheck
         
                };

                Employees.Add(
                    new EmployeeCardViewModel
                    { 
                        
                        Name = newDoctor.Name,
                        Salary = $"{newDoctor.Salary}$",
                        Department = newDoctor.Department.Name,
                        Role = "Doctor"
                    }
                   );

                FilteredEmployees.Add(
                   new EmployeeCardViewModel
                   {
                       Name = newDoctor.Name,
                       Salary = $"{newDoctor.Salary}$",
                       Department = newDoctor.Department.Name,
                       Role = "Doctor"
                   }
                  );

                Hospital.Employees.Add(newDoctor.ID, newDoctor);
                HospitalDB.InsertDoctor(newDoctor);
                //TODO UPDATE DOCTOR IN DB
            }
            else if(EmployeeRole=="Nurse")
            {
                Nurse newNurse = new Nurse
                {
                    Name = EmployeeNameTextBox,
                    Salary = Double.Parse(EmployeeSalaryTextBox),
                    Department = Hospital.Departments[EmployeeDepartment.Key],
                    Address = EmployeeAddressTextBox,
                };


                Employees.Add(
                    new EmployeeCardViewModel
                    {
                        Name = newNurse.Name,
                        Salary = $"{newNurse.Salary}$",
                        Department = newNurse.Department.Name,
                        Role = "Nurse"
                    }
                   );
               FilteredEmployees.Add(
                  new EmployeeCardViewModel
                  {
                      Name = newNurse.Name,
                      Salary = $"{newNurse.Salary}$",
                      Department = newNurse.Department.Name,
                      Role = "Nurse"
                  }
                 );

                Hospital.Employees.Add(newNurse.ID, newNurse);
                HospitalDB.InsertNurse(newNurse);
            }

        }
        

        public bool ValidateName()
        {
            EmployeeNameTextBox = (EmployeeNameTextBox != null) ? EmployeeNameTextBox.Trim() : "";
            EmployeeAddressTextBox = (EmployeeAddressTextBox != null) ? EmployeeAddressTextBox.Trim() : "";
            EmployeeSalaryTextBox = (EmployeeSalaryTextBox != null) ? EmployeeSalaryTextBox.Trim() : "";
            EmployeeRole = (EmployeeRole != null) ? EmployeeRole.Trim() : "";
            if (EmployeeNameTextBox == "") return false;
            if (EmployeeAddressTextBox == "") return false;
            if (EmployeeSalaryTextBox == "") return false;
            if (EmployeeDepartment == null) return false;
            if (EmployeeRole == "") return false;
            return true;
        }
    }
}
