﻿/*
      █▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀ ▀▀▀▀▀▀▀▀▀▀▀▀▀▀ ▀▀▀  ▀  ▀      ▀▀ 
      █   
      █      ▄███████▄                                                                ▄▄▄▄███▄▄▄▄                                                             
      █     ███    ███                                                              ▄██▀▀▀███▀▀▀██▄                                                           
      █     ███    ███    █████  ██████     ▄████▄     █████   ▄█████     ▄▄██▄▄▄   ███   ███   ███   ▄█████  ██▄▄▄▄    ▄█████     ▄████▄     ▄█████    █████ 
      █     ███    ███   ██  ██ ██    ██   ██    ▀    ██  ██   ██   ██  ▄█▀▀██▀▀█▄  ███   ███   ███   ██   ██ ██▀▀▀█▄   ██   ██   ██    ▀    ██   █    ██  ██ 
      █   ▀█████████▀   ▄██▄▄█▀ ██    ██  ▄██        ▄██▄▄█▀   ██   ██  ██  ██  ██  ███   ███   ███   ██   ██ ██   ██   ██   ██  ▄██        ▄██▄▄     ▄██▄▄█▀ 
      █     ███        ▀███████ ██    ██ ▀▀██ ███▄  ▀███████ ▀████████  ██  ██  ██  ███   ███   ███ ▀████████ ██   ██ ▀████████ ▀▀██ ███▄  ▀▀██▀▀    ▀███████ 
      █     ███          ██  ██ ██    ██   ██    ██   ██  ██   ██   ██  ██  ██  ██  ███   ███   ███   ██   ██ ██   ██   ██   ██   ██    ██   ██   █    ██  ██ 
      █    ▄████▀        ██  ██  ██████    ██████▀    ██  ██   ██   █▀   █  ██  █    ▀█   ███   █▀    ██   █▀  █   █    ██   █▀   ██████▀    ███████   ██  ██ 
      █   
 ▄ ▄▄ █ ▄▄▄▄▄▄▄▄▄  ▄▄▄▄ ▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄ ▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄ ▄▄  ▄▄ ▄▄   ▄▄▄▄ ▄▄     ▄▄     ▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄ ▄ ▄ 
 █ ██ █ █████████  ████ ██████████████████████████████████████ ███████████████ ██  ██ ██   ████ ██     ██     ████████████████ █ █ 
      █  
      █  The ProgramManager acts as a Service Locator and Dependency Injector for the application.
      █  
      ▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀  ▀▀ ▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀██ 
                                                                                                   ██ 
                                                                                               ▀█▄ ██ ▄█▀ 
                                                                                                 ▀████▀   
                                                                                                   ▀▀                            */
using System;
using System.Collections.Generic;
using NLog;
using System.Linq;
using System.Reflection;

namespace Symbiote.Core
{
    /// <summary>
    /// The ProgramManager acts as a Service Locator and Dependency Injector for the application.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     This class is a Singleton, however the static method which invokes the private constructor is itself private and is invoked via reflection.  This is 
    ///     by design so that Plugins and scripts are not easily capable of invoking the method; the preference is that dependencies are injected rather than 
    ///     retrieved from the Service Locator.
    /// </para>
    /// <para>
    ///     If dependency injection isn't feasible the ProgramManager instance can be retrieved using the <see cref="GetInstance()"/> method, and the individual
    ///     Managers can be retrieved using <see cref="GetManager{T}()"/>.
    /// </para>
    /// </remarks>
    public class ProgramManager : Manager, IStateful, IManager, IProgramManager
    {
        #region Variables

        /// <summary>
        /// The Logger for this class.
        /// </summary>
        new private static xLogger logger = (xLogger)LogManager.GetCurrentClassLogger(typeof(xLogger));

        /// <summary>
        /// The Singleton instance of ProgramManager.
        /// </summary>
        private static ProgramManager instance;

        #endregion

        #region Properties

        #region IProgramManager Properties

        /// <summary>
        /// Indicates whether the program is in Safe Mode.  Safe Mode is a sort of fault tolerant mode designed
        /// to allow the application to run under conditions that would otherwise raise fatal errors.
        /// </summary>
        public bool SafeMode { get; private set; }

        /// <summary>
        /// The name of the product, retrieved from AssemblyInfo.cs.
        /// </summary>
        public string ProductName { get { return System.Reflection.Assembly.GetExecutingAssembly().GetName().Name; } }

        /// <summary>
        /// The version of the product, retrieved from AssemblyInfo.cs.
        /// </summary>
        public Version ProductVersion { get { return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version; } }

        /// <summary>
        /// The name of the application instance.
        /// </summary>
        /// <remarks>
        ///     If the "InstanceName" setting is missing from the application settings, the value of the 
        ///     ProductName property is substituted.
        /// </remarks>
        public string InstanceName { get { return GetInstanceName(); } }

        #endregion

        /// <summary>
        /// The list of application Manager Types.
        /// </summary>
        private List<Type> ManagerTypes { get; set; }

        /// <summary>
        /// The list of application Manager instances.
        /// </summary>
        private List<IManager> ManagerInstances { get; set; }

        /// <summary>
        /// A dictionary containing a list of dependencies for each application Manager.
        /// </summary>
        private Dictionary<Type, List<Type>> ManagerDependencies { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// The default constructor.
        /// </summary>
        /// <param name="managerTypes">The array of Manager Types for the application.</param>
        /// <param name="safeMode">True if the ProgramManager is being started in Safe Mode, false otherwise.</param>
        private ProgramManager(Type[] managerTypes, bool safeMode = false)
        {
            base.logger = logger;
            Guid guid = logger.EnterMethod(xLogger.Params(managerTypes, safeMode), true);

            // initialize properties
            ManagerName = "Program Manager";
            ManagerTypes = managerTypes.ToList();
            ManagerInstances = new List<IManager>();
            ManagerDependencies = new Dictionary<Type, List<Type>>();

            // configure the SafeMode option
            SafeMode = safeMode;
            if (safeMode)
                logger.Info("Safe Mode enabled.  The program is now running in a limited fault tolerant mode.");

            // register the ProgramManager
            RegisterManager<ProgramManager>(this);


            //---------------------- -   ----------------------- -      --------  -    -
            // create an instance of each Manager Type in the ManagerTypes list
            logger.Debug("Instantiating Managers...");
            InstantiateManagers();
            logger.Debug("Managers instantiated successfully.");
            //------------------------------- -  -               ------------ 


            ChangeState(State.Initialized);

            logger.ExitMethod(guid);
        }

        /// <summary>
        /// Returns the singleton instance of the ProgramManager.  Creates an instance if null.
        /// </summary>
        /// <param name="managers">The array of Manager Types for the application.</param>
        /// <param name="safeMode">True if the ProgramManager is being started in Safe Mode, false otherwise.</param>
        /// <returns>The singleton instance of the ProgramManager</returns>
        internal static ProgramManager Instantiate(Type[] managers, bool safeMode = false)
        {
            if (instance == null)
                instance = new ProgramManager(managers, safeMode);

            return instance;
        }

        #endregion

        #region Instance Methods

        /// <summary>
        /// Executed upon startup of the Manager.  Starts all application managers.
        /// </summary>
        /// <returns>A Result containing the result of the operation.</returns>
        protected override Result Startup()
        {
            Guid guid = logger.EnterMethod(true);
            logger.Debug("Performing Startup for '" + GetType().Name + "'...");
            Result retVal = new Result();

            // start all application managers.
            retVal = StartManagers();

            retVal.LogResult(logger.Debug);
            logger.ExitMethod(guid);
            return retVal;
        }

        protected override Result Shutdown(StopType stopType = StopType.Normal, bool restartPending = false)
        {
            Guid guid = logger.EnterMethod(true);
            logger.Debug("Performing Shutdown for '" + GetType().Name + "'...");
            Result retVal = new Result();

            retVal.LogResult(logger.Debug);
            logger.ExitMethod(guid);
            return retVal;
        }

        /// <summary>
        /// Iterates over the list of Manager Types and instantiates each in the order in which they are represented in the list.
        /// </summary>
        /// <exception cref="MissingMethodException">Thrown when the 'InstantiateManager()' method can not be found.</exception>
        /// <exception cref="MissingMethodException">Thrown when the 'RegisterManager()' method can not be found.</exception>
        /// <exception cref="Exception">Thrown when the Manager instantiation returns an abnormal response.</exception>
        private void InstantiateManagers()
        {
            InstantiateManagers(ManagerTypes);
        }

        /// <summary>
        /// Iterates over the list of Manager Types and instantiates each in the order in which they are represented in the list.
        /// </summary>
        /// <exception cref="MissingMethodException">Thrown if the 'InstantiateManager()' method can not be found.</exception>
        /// <exception cref="MissingMethodException">Thrown if the 'RegisterManager()' method can not be found.</exception>
        /// <exception cref="Exception">Thrown when the Manager instantiation returns an abnormal response.</exception>
        private void InstantiateManagers(List<Type> managerTypes)
        {
            logger.EnterMethod();
            logger.Trace("Instantiating Managers...");

            // iterate over the list
            foreach (Type managerType in managerTypes)
            {
                logger.Separator(LogLevel.Debug);
                logger.Debug("Instantiating '" + managerType.Name + "'...");

                // find the InstantiateManager() method so that we can invoke it via reflection
                MethodInfo instantiateMethod = GetType().GetMethod("InstantiateManager", BindingFlags.NonPublic | BindingFlags.Instance);
                if (instantiateMethod == default(MethodInfo))
                    throw new MissingMethodException("Failed to find the 'InstantiateManager' method within the '" + GetType().Name + "' class.");

                // create a generic method using the current Type and invoke it
                MethodInfo genericInstantiateMethod = instantiateMethod.MakeGenericMethod(managerType);
                IManager manager = (IManager)genericInstantiateMethod.Invoke(this, null);

                // ensure the resulting IManager is valid
                if (manager != default(IManager))
                {
                    logger.Debug("Successfully instantiated '" + manager.GetType().Name + "'.  Registering...");

                    // find the RegisterManager() method and make sure it was found
                    MethodInfo registerMethod = GetType().GetMethod("RegisterManager", BindingFlags.NonPublic | BindingFlags.Instance, null, CallingConventions.Any, new Type[] { typeof(IManager) }, null);
                    if (registerMethod == default(MethodInfo))
                        throw new MissingMethodException("Failed to find the 'RegisterManager' method within the '" + GetType().Name + "' class.");

                    // create a generic method using the current Type and invoke it
                    MethodInfo genericRegisterMethod = registerMethod.MakeGenericMethod(managerType);
                    genericRegisterMethod.Invoke(this, new object[] { manager });

                    logger.Debug("Successfully registered '" + manager.GetType().Name + "'.");
                }
                else
                    throw new Exception("Instantiation of Manager '" + managerType.Name + "' returned an abnormal response.");
            }

            logger.ExitMethod();
        }

        /// <summary>
        /// Creates and returns an instance of the specified Manager Type.
        /// </summary>
        /// <typeparam name="T">The Type of the Manager to instantiate.</typeparam>
        /// <returns>The instantiated IManager.</returns>
        /// <exception cref="MissingMethodException">Thrown when the 'Instantiate()' method of the specified Manager Type can not be found.</exception>
        /// <exception cref="MissingMethodException">Thrown when the 'ResolveManagerDependencies()' method can not be found.</exception>
        /// <exception cref="Exception">Thrown when the application fails to resolve dependencies for the specified Manager Type.</exception>
        /// <exception cref="Exception">Thrown when the 'Instantiate()' method of the specified Manager Type returns a null value.</exception>
        /// <exception cref="Exception">Thrown when the object returned by 'Instantiate()' does not implement the IManager interface.</exception>
        /// <exception cref="Exception">Thrown if the instantiation fails.  See inner exception for details.</exception>
        private T InstantiateManager<T>() where T : IManager
        {
            logger.EnterMethod(xLogger.TypeParams(typeof(T)));
            logger.Trace("Creating new instance of '" + typeof(T).Name + "'...");

            T instance;

            try
            {
                // use reflection to locate the static Instantiate() method, then check to make sure it was found.
                MethodInfo method = typeof(T).GetMethod("Instantiate", BindingFlags.NonPublic | BindingFlags.Static);
                if (method == default(MethodInfo))
                    throw new MissingMethodException("Method 'Instantiate' not found in class '" + typeof(T).Name + "'.");

                // use reflection to locate the static ResolveManagerDependencies() method, then check to make sure it was found.
                MethodInfo resolveMethod = GetType().GetMethod("ResolveManagerDependencies", BindingFlags.NonPublic | BindingFlags.Instance);
                if (resolveMethod == default(MethodInfo))
                    throw new MissingMethodException("Method 'ResolveManagerDependencies' was not found in class '" + GetType().Name + "'.");

                // make a generic version of ResolveManagerDependencies() using the specified type, then invoke it and
                // check the result to ensure it is valid.
                MethodInfo genericResolveMethod = resolveMethod.MakeGenericMethod(typeof(T));
                List<IManager> resolvedDependencies = (List<IManager>)genericResolveMethod.Invoke(this, new object[] { });
                if (resolvedDependencies == default(List<IManager>))
                    throw new Exception("Failed to resolve Manager dependencies for '" + typeof(T) + "'.");

                // invoke the Instanctiate() method and pass the resolved dependencies from the step above.
                // store the result in instance, then check to make sure it is not null and ensure that it implements IManager.
                instance = (T)method.Invoke(null, resolvedDependencies.ToArray());
                if (instance == null)
                    throw new Exception("Instantiate() method invocation from '" + typeof(T).Name + "' returned no result.");
                else if (!(instance is IManager))
                    throw new Exception("The instance returned by Instantiate() method invocation from '" + typeof(T).Name + "' does not implement the IManager interface.");

                logger.Trace("Successfully instantiated '" + instance.GetType().Name + "'.");
            }
            catch (Exception ex)
            {
                logger.Exception(ex);
                throw new Exception("Failed to instantiate Manager '" + typeof(T).Name + "': " + ex.Message, ex);
            }

            logger.ExitMethod();
            return instance;
        }

        /// <summary>
        /// Retrieves the list of <see cref="Type"/>s corresponding to the Managers on which the specified Manager Type depends.
        /// </summary>
        /// <remarks>
        ///     Uses reflection to retrieve the parameters for the constructor of the specified Type.  To dictate dependencies for a 
        ///     Manager, simply declare the dependent Types as parameters on the 
        /// </remarks>
        /// <typeparam name="T">The Type of the Manager for which the dependencies are to be returned.</typeparam>
        /// <returns>A List of dependency Types.</returns>
        /// <exception cref="Exception">Thrown when an exception is caught while retrieving the Instantiate() parameters for the specified Type.</exception>
        private List<Type> GetManagerDependencies<T>() where T : IManager
        {
            logger.EnterMethod(xLogger.TypeParams(typeof(T)));
            logger.Trace("Retrieving dependencies for '" + typeof(T).Name + "'...");

            List<Type> retVal = new List<Type>();

            // the ProgramManager has no dependencies (that we need to track).
            if (typeof(T) == GetType()) return retVal;

            try
            {
                // retrieve the list of parameters for the Instantiate() method of the specified Manager Type and add the type of each
                // to the return value list
                ParameterInfo[] parameters = typeof(T).GetMethod("Instantiate", BindingFlags.NonPublic | BindingFlags.Static).GetParameters();

                foreach (ParameterInfo p in parameters)
                    retVal.Add(p.ParameterType);
            }
            catch (Exception ex)
            {
                logger.Exception(ex);
                throw new Exception("Failed to retrieve dependencies for '" + typeof(T).Name + "': " + ex.Message, ex);
            }

            logger.Trace("Retrieved " + retVal.Count() + " dependenc" + (retVal.Count() == 1 ? "y" : "ies") + ".");

            logger.ExitMethod(retVal);
            return retVal;
        }

        /// <summary>
        /// Returns a list of IManager instances corresponding to the Manager Types upon which the specified Manager is dependent.
        /// </summary>
        /// <typeparam name="T">The Manager Type for which the dependencies are to be resolved.</typeparam>
        /// <returns>A list of IManager instances corresponding to the Manager Types upon which the specified Manager is dependent.</returns>
        /// <exception cref="MissingMethodException">Thrown when the 'GetManager()' method can not be found.</exception>
        /// <exception cref="Exception">Thrown when the 'GetManagerDependencies()' method returns a null or empty list.</exception>
        /// <exception cref="Exception">Thrown when the invocation of the 'GetManager{T}()' method fails.  See inner exception for details.</exception>
        /// <exception cref="Exception">Thrown when the invocation of the 'GetManager{T}()' method returns a null instance of IManager.</exception>
        /// <exception cref="Exception">Thrown when an exception is caught while resolving the dependencies for the specified Manager Type.</exception>
        private List<IManager> ResolveManagerDependencies<T>() where T : IManager
        {
            logger.EnterMethod(xLogger.TypeParams(typeof(T)));
            logger.Trace("Resolving dependencies for '" + typeof(T).Name + "'...");

            List<IManager> retVal = new List<IManager>();

            try
            {
                // find the GetManager() method and check to ensure it was found
                MethodInfo getManager = GetType().GetMethod("GetManager", BindingFlags.Public | BindingFlags.Instance);
                if (getManager == default(MethodInfo))
                    throw new MissingMethodException("Method 'GetManager' was not found in class '" + GetType().Name + "'.");

                MethodInfo getManagerGeneric;

                // retrieve dependencies for the specified Manager
                // any instance of IManager needs to have at least one dependency (ProgramManager).  If we get a null or empty list something is wrong.
                List<Type> dependencies = GetManagerDependencies<T>();
                if ((dependencies == default(List<Type>)) || (dependencies.Count() == 0))
                    throw new Exception("Failed to retrieve dependencies for '" + typeof(T).Name + "'.  Method 'GetManagerDependencies()' returned a null or empty List.");

                // iterate over the dependencies
                foreach (Type t in dependencies)
                {
                    logger.Trace("Attempting to resolve dependency '" + t.Name + "'...");

                    IManager manager;

                    getManagerGeneric = getManager.MakeGenericMethod(t);

                    try
                    {
                        manager = (IManager)getManagerGeneric.Invoke(this, new object[] { });
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Invocation of method 'GetManager<T>' failed: " + ex.Message, ex);
                    }

                    if ((IManager)manager == default(IManager))
                        throw new Exception("Invocation of method 'GetManager<T>' returned a null instance of IManager.");

                    logger.Trace("Successfully resolved depencency '" + t.Name + "'.");
                    retVal.Add(manager);
                }

            }
            catch (Exception ex)
            {
                logger.Exception(ex);
                throw new Exception("Failed to resolve dependencies for '" + typeof(T).Name + "': " + ex.Message, ex);
            }

            logger.Trace("Resolved " + retVal.Count() + " dependenc" + (retVal.Count() == 1 ? "y" : "ies" + "."));

            logger.ExitMethod(retVal.Select(d => d.GetType()));
            return retVal;
        }

        /// <summary>
        /// Adds the specified Manager to the <see cref="ManagerInstances"/> list and subscribes to its StateChanged event.
        /// </summary>
        /// <typeparam name="T">The Type of the specified Manager.</typeparam>
        /// <param name="manager">The Manager to register.</param>
        /// <returns>The registered Manager.</returns>
        /// <exception cref="Exception">Thrown if the specified Manager has already been registered.</exception>
        /// <exception cref="Exception">Thrown if the dependency list retrieved for the Manager is empty.</exception>
        /// <exception cref="Exception">Thrown if the registration fails.</exception>
        private T RegisterManager<T>(IManager manager) where T : IManager
        {
            return RegisterManager<T>(manager, ManagerInstances, ManagerDependencies);
        }

        /// <summary>
        /// Adds the specified Manager to the Manager list and subscribes to its StateChanged event.
        /// </summary>
        /// <typeparam name="T">The Type of the specified Manager.</typeparam>
        /// <param name="manager">The Manager to register.</param>
        /// <param name="managerInstances">The list of Managers to which the Manager is to be registered.</param>
        /// <param name="managerDependencies">The dictionary containing the list of dependencies for each application Manager.</param>
        /// <returns>The registered Manager.</returns>
        /// <exception cref="Exception">Thrown if the specified Manager has already been registered.</exception>
        /// <exception cref="Exception">Thrown if the dependency list retrieved for the Manager is empty.</exception>
        /// <exception cref="Exception">Thrown if the registration fails.</exception>
        private T RegisterManager<T>(IManager manager, List<IManager> managerInstances, Dictionary<Type, List<Type>> managerDependencies) where T : IManager 
        {
            logger.EnterMethod(xLogger.TypeParams(typeof(T)), xLogger.Params(manager, new xLogger.ExcludedParam(), new xLogger.ExcludedParam()));
            logger.Trace("Registering Manager '" + manager.GetType().Name + "'...");

            // ensure the specified Manager hasn't already been registered.  There can only be one of each Type
            // in the Manager list.
            if (IsRegistered<T>())
                throw new Exception("The Manager '" + manager.GetType().Name + "' is already registered.");

            try
            {
                // retrieve the dependencies for the Manager
                List<Type> dependencies = GetManagerDependencies<T>();

                if (dependencies == default(List<Type>))
                    throw new Exception("The dependency list for the Manager '" + manager.GetType().Name + "' is empty; all Managers must have at least one dependency.");

                logger.Trace("Registering Manager with " + dependencies.Count() + " dependencies...");

                // add the specified Manager to the list and attach an event handler to its StateChanged event
                managerInstances.Add(manager);
                managerDependencies.Add(typeof(T), dependencies);
                manager.StateChanged += ManagerStateChanged;
            }
            catch (Exception ex)
            {
                logger.Exception(ex);
                throw new Exception("Failed to register Manager '" + manager.GetType().Name + "': " + ex.Message, ex);
            }

            logger.Trace("Successfully registered Manager '" + manager.GetType().Name + "'.");

            logger.ExitMethod();
            return (T)manager;
        }

        /// <summary>
        /// Returns the Manager from the list of Managers matching the specified Type.
        /// </summary>
        /// <typeparam name="T">The Type of the Manager to return.</typeparam>
        /// <returns>The requested Manager.</returns>
        public T GetManager<T>() where T : IManager
        {
            return GetManager<T>(ManagerInstances);
        }

        /// <summary>
        /// Returns the Manager from the list of Managers matching the specified Type.
        /// </summary>
        /// <typeparam name="T">The Type of the Manager to return.</typeparam>
        /// <param name="managers">The list of Managers from which to retrieve the specified Manager Type.</param>
        /// <returns>The requested Manager.</returns>
        private T GetManager<T>(List<IManager> managers) where T : IManager
        {
            return managers.OfType<T>().FirstOrDefault();
        }

        /// <summary>
        /// Returns a list of Manager Types for which the ManagerDependencies dictionary contains an entry for the specified Manager Type.
        /// </summary>
        /// <typeparam name="T">The Manager Type for which the dependent Types are to be returned.</typeparam>
        /// <returns>The list of Manager Types for which the ManagerDependencies dictionary contains an entry for the specified Manager Type.</returns>
        private List<Type> GetManagerDependentTypes<T>() where T : IManager
        {
            return GetManagerDependentTypes<T>(ManagerDependencies);
        }

        /// <summary>
        /// Returns a list of Manager Types for which the specfied dependency dictionary contains an entry for the specified Manager Type.
        /// </summary>
        /// <typeparam name="T">The Manager Type for which the dependent Types are to be returned.</typeparam>
        /// <param name="managerDependencies">The dictionary of Manager dependencies to search.</param>
        /// <returns>The list of Manager Types for which the ManagerDependencies dictionary contains an entry for the specified Manager Type.</returns>
        private List<Type> GetManagerDependentTypes<T>(Dictionary<Type, List<Type>> managerDependencies) where T : IManager
        {
            List<Type> retVal = new List<Type>();

            foreach (Type key in managerDependencies.Keys)
                if (managerDependencies[key].Where(t => t.IsAssignableFrom(typeof(T))).Count() > 0)
                    retVal.Add(key);

            return retVal;
        }

        /// <summary>
        /// Returns true if the specified Manager Type is registered, false otherwise.
        /// </summary>
        /// <typeparam name="T">The Manager Type to check.</typeparam>
        /// <returns>True if the specified Manager Type is registered, false otherwise.</returns>
        public bool IsRegistered<T>() where T : IManager
        {
            return IsRegistered<T>(ManagerInstances);
        }

        /// <summary>
        /// Returns true if the specified Manager Type is registered, false otherwise.
        /// </summary>
        /// <typeparam name="T">The Manager Type to check.</typeparam>
        /// <param name="managers">The list of Managers to check.</param>
        /// <returns>True if the specified Manager Type is registered, false otherwise.</returns>
        private bool IsRegistered<T>(List<IManager> managers) where T : IManager
        {
            return managers.OfType<T>().Count() > 0;
        }

        /// <summary>
        /// Starts each Manager contained within the <see cref="ManagerInstances"/> list.
        /// </summary>
        /// <returns>A Result containing the result of the operation.</returns>
        private Result StartManagers()
        {
            logger.EnterMethod();

            Result retVal = StartManagers(ManagerInstances);

            logger.ExitMethod(retVal);
            return retVal;
        }

        /// <summary>
        /// Starts each Manager contained within the specified list of Manager instances.
        /// </summary>
        /// <remarks>
        /// Does not Start the ProgramManager instance.
        /// </remarks>
        /// <param name="managerInstances">The list of Manager instances to start.</param>
        /// <returns>A Result containing the result of the operation.</returns>
        private Result StartManagers(List<IManager> managerInstances)
        {
            Guid guid = logger.EnterMethod(xLogger.Params(managerInstances), true);

            logger.Debug("Starting Managers...");

            Result retVal = new Result();

            // iterate over the Manager instance list and start each manager.
            // skip the ProgramManager as it has already been started.
            foreach (IManager manager in managerInstances)
            {
                if (manager != this)
                {
                    logger.SubHeading(LogLevel.Debug, manager.GetType().Name);
                    retVal.Incorporate(StartManager(manager));

                    if (retVal.ResultCode == ResultCode.Failure)
                        return retVal.AddError("Failed to start one or more Managers.");
                }
            }

            retVal.LogResult(logger);
            logger.ExitMethod(retVal, guid);
            return retVal;
        }

        private Result StopManagers(StopType stopType = StopType.Normal)
        {
            logger.EnterMethod();

            Result retVal = StopManagers(stopType, ManagerInstances);

            logger.ExitMethod(retVal);
            return retVal;
        }

        /// <summary>
        /// Stops each of the <see cref="IManager"/> instances in <see cref="ManagerInstances"/>.
        /// </summary>
        /// <remarks>
        /// Does not Stop the ProgramManager instance.
        /// </remarks>
        /// <param name="stopType">The type of stoppage.</param>
        /// <param name="managerInstances">The list of Manager instances to stop.</param>
        /// <returns>A Result containing the result of the operation.</returns>
        private Result StopManagers(StopType stopType, List<IManager> managerInstances)
        {
            Guid guid = logger.EnterMethod(xLogger.Params(stopType, managerInstances));
            logger.Debug("Stopping Managers...");
            Result retVal = new Result();

            // iterate over the Manager instance list in reverse order, stopping each manager.
            // skip the ProgramManager as it will stop when this process is complete.
            for (int i = managerInstances.Count(); i <= 0; i--)
                if (managerInstances[i] != this)
                    retVal.Incorporate(StopManager(managerInstances[i]));

            retVal.LogResult(logger);
            logger.ExitMethod(retVal, guid);
            return retVal;
        }

        /// <summary>
        /// Starts the specified IManager instance
        /// </summary>
        /// <param name="manager">The IManager instance to start.</param>
        /// <returns>A Result containing the result of the operation and the specified IManager instance.</returns>
        internal Result<IManager> StartManager(IManager manager)
        {
            Guid guid = logger.EnterMethod(xLogger.Params(manager), true);
            logger.Debug("Starting " + manager.GetType().Name + "...");
            Result<IManager> retVal = new Result<IManager>();

            // invoke the Start() method on the specified manager
            Result startResult = manager.Start();

            // if the manager fails to start, throw an exception and halt the program
            if (startResult.ResultCode == ResultCode.Failure)
                retVal.AddError("Failed to start Manager '" + manager.GetType().Name + "'.");

            retVal.ReturnValue = manager;
            retVal.Incorporate(startResult);

            if (retVal.ResultCode != ResultCode.Failure)
                logger.Debug("Successfully started " + manager.GetType().Name + ".");
            else
                logger.Debug("Failed to start " + manager.GetType().Name + ": " + retVal.LastErrorMessage());

            retVal.LogResult(logger.Debug);
            logger.ExitMethod(retVal, guid);
            return retVal;
        }

        /// <summary>
        /// Stops the specified IManager instance.
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="stopType"></param>
        /// <returns></returns>
        internal Result StopManager(IManager manager, StopType stopType = StopType.Normal)
        {
            Guid guid = logger.EnterMethod(xLogger.Params(manager, stopType), true);

            logger.Debug("Stopping " + manager.GetType().Name + "...");

            Result retVal = manager.Stop(stopType);

            if (retVal.ResultCode == ResultCode.Failure)
                retVal.AddError("Failed to stop " + manager.GetType().Name + "." + retVal.LastErrorMessage());

            retVal.LogResult(logger);
            logger.ExitMethod(retVal, guid);
            return retVal;
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Event handler for the StateChanged event of registered Managers.
        /// </summary>
        /// <param name="sender">The Manager which fired the event.</param>
        /// <param name="e">The EventArgs for the event.</param>
        /// <exception cref="MissingMethodException">Thrown if the method 'GetManagerDependentTypes()' can not be found.</exception>"
        private void ManagerStateChanged(object sender, StateChangedEventArgs e)
        {
            logger.Info("Manager '" + sender.GetType().Name + "' state changed from '" + e.PreviousState + "' to '" + e.State + "'." + (e.Message != "" ? "(" + e.Message + ")" : ""));
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Returns the Singleton instance of ProgramManager
        /// </summary>
        /// <remarks>
        /// Use only in situations where dependency injection is not feasible.
        /// </remarks>
        /// <returns>The Singleton instance of ProgramManager</returns>
        /// <exception cref="Exception">Thrown when the method is invoked prior to the ProgramManager having been instantiated.</exception>
        internal static ProgramManager GetInstance()
        {
            if (instance == null)
                throw new Exception("Failed to retrieve the ProgramManager instance; it has not yet been instantiated.");

            return instance;
        }

        /// <summary>
        /// Returns the "InstanceName" setting from the app.config file, or the default value if the setting is not retreived.
        /// </summary>
        /// <returns>The name of the program instance.</returns>
        public static string GetInstanceName()
        {
            return Utility.GetSetting("InstanceName", "Symbiote");
        }

        #endregion
    }
}
