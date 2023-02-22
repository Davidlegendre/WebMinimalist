using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
using System.Windows.Shapes;
using WebBrowserMinimalist.Services;
using WebBrowserMinimalist.ViewModels;
using Wpf.Ui.Controls.Interfaces;
using Wpf.Ui.Mvvm.Contracts;

namespace WebBrowserMinimalist.Views.Controls.Collections_Windows
{
    /// <summary>
    /// Lógica de interacción para AgregarColeccionWindow.xaml
    /// </summary>
    public partial class AgregarColeccionWindow : INavigationWindow
    {
        readonly CollectionVM _collectionVm;
        TypeDataTranfered dataTranfered;
        public AgregarColeccionWindow(Window Owner, CollectionVM collectionVM,TypeDataTranfered typeDataTranfered = TypeDataTranfered.Add)
        {
            InitializeComponent();
            this.Owner = Owner;
            _collectionVm = collectionVM;
            dataTranfered = typeDataTranfered;
            cambiarInterfaz();
        }

        void cambiarInterfaz()
        {
            switch(dataTranfered)
            {
                case TypeDataTranfered.Add:
                    txtBtnAddOrMod.Text = "Crear Coleccion";
                    SymbolAddOrMod.Symbol = Wpf.Ui.Common.SymbolRegular.Add24;
                    break;
                case TypeDataTranfered.Edit:
                    txtBtnAddOrMod.Text = "Editar Coleccion";
                    SymbolAddOrMod.Symbol = Wpf.Ui.Common.SymbolRegular.Edit24;
                    break;
            }
        }

        public void CloseWindow()
        {
            throw new NotImplementedException();
        }

        public Frame GetFrame()
        {
            throw new NotImplementedException();
        }

        public INavigation GetNavigation()
        {
            throw new NotImplementedException();
        }

        public bool Navigate(Type pageType)
        {
            throw new NotImplementedException();
        }

        public void SetPageService(IPageService pageService)
        {
            throw new NotImplementedException();
        }

        public void ShowWindow()
        {
            throw new NotImplementedException();
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void btnAddOrMod_Click(object sender, RoutedEventArgs e)
        {
            if (IsAnyNotVoid())
            {
                if (MessageBox.Show("Desea " + (dataTranfered == TypeDataTranfered.Add ? "agregar " : " modificar ") + " la coleccion?",
                    (dataTranfered == TypeDataTranfered.Add ? "Crear": "Modificar"), MessageBoxButton.YesNo) == MessageBoxResult.Yes) {
                    var selectColor = (Wpf.Ui.Controls.CardColor)listColor.SelectedItem;
                    switch (dataTranfered)
                    {
                        case TypeDataTranfered.Add:
                            var result = await _collectionVm.CreateCollection(new Models.CollectionsModel()
                            {
                                ID = Guid.NewGuid().ToString(),
                                TituloColeccion = txtTitulo.Text,
                                Background = selectColor.Color.ToString()
                            });
                            if (result)
                                this.Close();
                            break;
                        case TypeDataTranfered.Edit:
                            var id = this.Tag as string;
                            var result2 = await _collectionVm.UpdateCollection(new Models.CollectionsModel()
                            {
                                ID = id,
                                TituloColeccion = txtTitulo.Text,
                                Background = selectColor.Color.ToString()
                            });
                            if (result2)
                                this.Close();
                            break;
                    }
                    
                }
               
            }
            
        }

        bool IsAnyNotVoid()
        {
            bool result = true;
            if(string.IsNullOrWhiteSpace(txtTitulo.Text))
            {
                MessageBox.Show("Escriba algun Titulo para la coleccion");
                txtTitulo.Focus();
                result = false;
            }

            if (listColor.SelectedIndex == -1) {
                MessageBox.Show("Seleccione algun Color");
                result = false;
            }
            return result;
        }
    }

    public enum TypeDataTranfered
    { 
        Add,
        Edit
    }
}
