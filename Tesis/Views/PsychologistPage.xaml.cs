﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tesis.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Tesis.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PsychologistPage : ContentPage
    {

        public PsychologistPage()
        {
            
            InitializeComponent();
           BindingContext = new PsychologistViewModel();
            
        }
    }
}