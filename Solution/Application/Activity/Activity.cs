﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Threading;

namespace Genlog
{
    public class ViewChangedArgs : EventArgs
    {
        public UserControl RequestedView
        {
            get;
            set;
        }
    }

    public abstract class Activity : IStoppable, IStartable
    {
        
        private MainWindow Context { get; set; }

        private Dictionary<string, UserControl> _views;
        public UserControl CurrentView { get; private set; }

        public event ViewChangedHandler OnViewChanged;
        public delegate void ViewChangedHandler(Activity sender, ViewChangedArgs e);

        public Activity(MainWindow context)
        {
            Context = context;
            _views = new Dictionary<string, UserControl>();
            SetLanguageDictionary();
            Console.WriteLine("Activity::Create called");
        }

        public virtual void Start()
        {
            Console.WriteLine("Activity::Start called");
            
            StartView();
        }

        public virtual void Stop()
        {
            Console.WriteLine("Activity::Stop called");

            StopView();
        }

        private void StartView()
        {
            IStartable startableView = CurrentView as IStartable;

            if (startableView != null)
                startableView.Start();
        }

        private void StopView()
        {
            IStoppable stoppableView = CurrentView as IStoppable;

            if (stoppableView != null)
                stoppableView.Stop();
        }

        protected void AddView(string id, UserControl view)
        {
            _views.Add(id, view);
        }

        public void Show(string id)
        {
            // si la vue existe
            if (_views.ContainsKey(id))
            {
                CurrentView = _views[id];
                
                StartView();
                
                if (OnViewChanged != null)
                    OnViewChanged(this, new ViewChangedArgs() { RequestedView = CurrentView });
            }
        }


        private void SetLanguageDictionary()
        {
            ResourceDictionary dict = new ResourceDictionary();
            switch (Thread.CurrentThread.CurrentCulture.ToString())
            {
                case "en-US":
                    dict.Source = new Uri("../../Resources/StringResources.en.xaml",
                                  UriKind.Relative);
                    break;
                case "fr-CA":
                    dict.Source = new Uri("../../Resources/StringResources.fr.xaml",
                                       UriKind.Relative);
                    break;
                default:
                    dict.Source = new Uri("../../Resources/StringResources.fr.xaml",
                                      UriKind.Relative);
                    break;
            }
            Application.Current.Resources.MergedDictionaries.Add(dict);
        } 

    }
}
