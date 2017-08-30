using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Deployment.Application;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sistema.Generales
{
    public class BuscarActualizaciones
    {
        long sizeOfUpdate = 0;
        private TextBox downloadStatus;
        bool flag = false;

         public  void UpdateApplication(TextBox dwdtxt)
        {
            this.downloadStatus = dwdtxt;
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                this.flag = false;
                ApplicationDeployment ad = ApplicationDeployment.CurrentDeployment;
                ad.CheckForUpdateCompleted += new CheckForUpdateCompletedEventHandler(ad_CheckForUpdateCompleted);
                ad.CheckForUpdateProgressChanged += new DeploymentProgressChangedEventHandler(ad_CheckForUpdateProgressChanged);

                ad.CheckForUpdateAsync();
            }
        }

        void ad_CheckForUpdateProgressChanged(object sender, DeploymentProgressChangedEventArgs e)
        {
            downloadStatus.Text = String.Format("Descargando: {0}. {1:D}K de {2:D}K descargados.", GetProgressString(e.State), e.BytesCompleted / 1024, e.BytesTotal / 1024);
        }

        string GetProgressString(DeploymentProgressState state)
        {
            if (state == DeploymentProgressState.DownloadingApplicationFiles)
            {
                return "application files";
            }
            else if (state == DeploymentProgressState.DownloadingApplicationInformation)
            {
                return "application manifest";
            }
            else
            {
                return "deployment manifest";
            }
        }

        void ad_CheckForUpdateCompleted(object sender, CheckForUpdateCompletedEventArgs e)
        {
            if (!this.flag)
            {
                if (e.Error != null)
                {
                    this.flag = true;
                    MessageBox.Show("ERROR: No se puedo Recuperar la nueva version de la Aplicación. Razon: \n" + e.Error.Message + "\nFavor de ponerse en contacto con el Administrador del Sistema.");
                    return;
                }
                else if (e.Cancelled == true)
                {
                    this.flag = true;
                    MessageBox.Show("La Actualización fue cancelada.");
                }

                // Ask the user if they would like to update the application now.
                if (e.UpdateAvailable)
                {
                    sizeOfUpdate = e.UpdateSizeBytes;

                    if (!e.IsUpdateRequired)
                    {
                        DialogResult dr = MessageBox.Show("Una actualización esta disponible. ¿Le gustaría actualizar la aplicación Ahora?\n\nTiempo Estimado de Descarga: ", "Actualización Disponible", MessageBoxButtons.OKCancel);
                        if (DialogResult.OK == dr)
                        {
                            this.flag = true;
                            BeginUpdate();
                        }
                        this.flag = true;
                    }
                    else
                    {
                        this.flag = true;
                        MessageBox.Show("Una actualización obligatoria está disponible para su aplicación. Se instalará la actualización ahora, la aplicación se reiniciará.");                       
                        BeginUpdate();
                    }
                }

            }
                
        }

        private void BeginUpdate()
        {
            ApplicationDeployment ad = ApplicationDeployment.CurrentDeployment;
            ad.UpdateCompleted += new AsyncCompletedEventHandler(ad_UpdateCompleted);

            // Indicate progress in the application's status bar.
            ad.UpdateProgressChanged += new DeploymentProgressChangedEventHandler(ad_UpdateProgressChanged);
            ad.UpdateAsync();
        }

        void ad_UpdateProgressChanged(object sender, DeploymentProgressChangedEventArgs e)
        {
            String progressText = String.Format("{0:D}K de {1:D}K descargados - {2:D}% completo", e.BytesCompleted / 1024, e.BytesTotal / 1024, e.ProgressPercentage);
            downloadStatus.Text = progressText;
        }

        void ad_UpdateCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                MessageBox.Show("La actualización a la versión mas reciente de la aplicación fue cancelada.");
                return;
            }
            else if (e.Error != null)
            {
                MessageBox.Show("ERROR: No se pudo instalar la versión mas reciente de la aplicación. Rason: \n" + e.Error.Message + "\nFavor de reportar este error al Administrador del sistema.");
                return;
            }

            DialogResult dr = MessageBox.Show("La aplicación ha sido actualizada. Reiniciar? (Si no reinicias ahora, los cambios no tendran efecto hasta que cierre y vuelva a abrir la aplicación.)", "Reiniciar Aplicación", MessageBoxButtons.OKCancel);
            if (DialogResult.OK == dr)
            {
                Application.Restart();
            }
        }
    }
}
