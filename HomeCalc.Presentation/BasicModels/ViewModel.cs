﻿using HomeCalc.Core;
using HomeCalc.Core.LogService;
using HomeCalc.Core.Presentation;
using HomeCalc.Model.DbService;
using HomeCalc.Presentation.Models;
using HomeCalc.Presentation.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HomeCalc.Presentation.BasicModels
{
    public partial class ViewModel
    {
        protected Logger logger;
        public event PropertyChangedEventHandler PropertyChanged;

        protected StorageService StoreService;
        protected StatusService Status;
        public ViewModel()
        {
            InitializeServices();
            InitializeProperties();
        }

        /*This method will be called after all initial data loading is finished*/
        protected virtual void Initialize()
        { }

        private void InitializeServices()
        {
            logger = new Logger(this.GetType().ToString());
            StoreService = StorageService.GetInstance();
            Status = StatusService.GetInstance();
        }

        
        private IDictionary<string, ICommand> commandCache = new Dictionary<string, ICommand>();
        public void AddCommand(string name, ICommand command)
        {
            if (string.IsNullOrEmpty(name) || command == null)
            {
                throw new ArgumentNullException("command");
            }
            if (commandCache.Keys.Contains(name))
            {
                throw new ArgumentException(string.Format("Command {0} already exists", name));
            }
            commandCache[name] = command;
        }
        public void RemoveCommand(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("command");
            }
            commandCache.Remove(name);
        }
    }
}
