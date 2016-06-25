﻿using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Symbiote.Core.OperationResult;

namespace Symbiote.Core.Plugin
{
    class PluginTypeManager<T> : IManager
    {
        #region Variables

        /// <summary>
        /// The Logger for this class.
        /// </summary>
        private static xLogger logger = (xLogger)LogManager.GetCurrentClassLogger(typeof(xLogger));

        /// <summary>
        /// The ProgramManager for the application.
        /// </summary>
        private ProgramManager manager;

        #endregion

        #region Properties

        #region IManager Implementation

        /// <summary>
        /// The state of the Manager.
        /// </summary>
        public ManagerState State { get; private set; }

        #endregion

        /// <summary>
        /// The list of Plugin Instances from the Plugin Manager Configuration which are managed by this Plugin Type Manager.
        /// </summary>
        public List<PluginManagerConfigurationPluginInstance> ConfiguredInstances { get; private set; }

        /// <summary>
        /// The list of Plugin Instances managed by this Plugin Type Manager.
        /// </summary>
        public List<T> Instances { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// The default constructor.
        /// </summary>
        /// <param name="manager">The ProgramManager instance for the application.</param>
        /// <param name="configuredInstances">A list of Plugin Instances from the Plugin Manager configuration which match this Plugin Type.</param>
        public PluginTypeManager(ProgramManager manager, List<PluginManagerConfigurationPluginInstance> configuredInstances)
        {
            this.manager = manager;
            ConfiguredInstances = configuredInstances;
        }

        #endregion

        #region Instance Methods

        #region IManager Implementation

        /// <summary>
        /// Starts the Manager.
        /// </summary>
        /// <returns>An Result containing the result of the operation.</returns>
        public Result Start()
        {
            return new Result();
        }

        /// <summary>
        /// Restarts the Manager.
        /// </summary>
        /// <returns>An Result containing the result of the operation.</returns>
        public Result Restart()
        {
            return new Result();
        }

        /// <summary>
        /// Stops the Manager.
        /// </summary>
        /// <returns>An Result containing the result of the operation.</returns>
        public Result Stop()
        {
            return new Result();
        }

        #endregion

        #endregion
    }
}
