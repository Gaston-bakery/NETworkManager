﻿using System;
using System.Threading.Tasks;
using Octokit;
using NETworkManager.Models.Settings;

namespace NETworkManager.Models.Update
{
    public class Updater
    {
        #region Events
        public event EventHandler<UpdateAvailableArgs> UpdateAvailable;

        protected virtual void OnUpdateAvailable(UpdateAvailableArgs e)
        {
            UpdateAvailable?.Invoke(this, e);
        }

        public event EventHandler NoUpdateAvailable;

        protected virtual void OnNoUpdateAvailable()
        {
            NoUpdateAvailable?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler ClientIncompatibleWithNewVersion;

        protected virtual void OnClientIncompatibleWithNewVersion()
        {
            ClientIncompatibleWithNewVersion?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler Error;

        protected virtual void OnError()
        {
            Error?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region Methods
        public void Check()
        {
            Task.Run(() =>
            {
                try
                {
                    var client = new GitHubClient(new ProductHeaderValue(Properties.Resources.NETworkManager_ProjectName));

                    var latestVersion = new Version(client.Repository.Release.GetLatest(Properties.Resources.NETworkManager_GitHub_User, Properties.Resources.NETworkManager_GitHub_Repo).Result.TagName.TrimStart('v'));

                    // Compare versions (tag=v2.0.1, version=2.0.1)
                    if (latestVersion > AssemblyManager.Current.Version)
                        OnUpdateAvailable(new UpdateAvailableArgs(latestVersion));
                    else
                        OnNoUpdateAvailable();
                }
                catch
                {
                    OnError();
                }
            });
        }
        #endregion
    }
}
