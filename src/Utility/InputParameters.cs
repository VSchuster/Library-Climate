//  Copyright: Portland State University 2009-2014
//  Authors:  Robert M. Scheller, Amin Almassian

using Edu.Wisc.Forest.Flel.Util;

namespace Landis.Library.Climate
{
    /// <summary>
    /// The parameters for biomass succession.
    /// </summary>
    public class InputParameters
         : IInputParameters
       
    {

        private string climateConfigFile;
        private string climateTimeSeries;
        private string climateFileFormat;
        private string climateFile;
        private string spinUpClimateFileFormat;
        private string spinUpClimateFile;
        private string spinUpClimateTimeSeries;
        // Added for Fire Climate output
        private bool fireClimate;
        private double rHSlopeAdjust;
        private int springStart;
        private int winterStart;


        //---------------------------------------------------------------------
        public string ClimateConfigFile
        {
            get
            {
                return climateConfigFile;
            }
            set
            {

                climateConfigFile = value;
            }
        }
        
        //---------------------------------------------------------------------
        /// <summary>
        /// Path to the required file with climatedata.
        /// </summary>
        /// 

        public string ClimateTimeSeries
        {
            get
            {
                return climateTimeSeries;
            }
            set
            {

                climateTimeSeries = value;
            }
        }

        public string ClimateFile
        {
            get {
                return climateFile;
            }
            set {
                string path = value;
                if (path.Trim(null).Length == 0)
                    throw new InputValueException(path, "\"{0}\" is not a valid path.", path);
                climateFile = value;
            }
        }

        public string ClimateFileFormat
        {
            get
            {
                return climateFileFormat;
            }
            set
            {

                climateFileFormat = value;
            }
        }

        public string SpinUpClimateTimeSeries  
        {
            get
            {
                return spinUpClimateTimeSeries;
            }
            set
            {

                spinUpClimateTimeSeries = value;
            }
        }

        public string SpinUpClimateFileFormat
        {
            get
            {
                return spinUpClimateFileFormat;
            }
            set
            {

                spinUpClimateFileFormat = value;
            }
        }

        public string SpinUpClimateFile			
        {
            get
            {
                return spinUpClimateFile;
            }
            set
            {
                string path = value;
                if (spinUpClimateFileFormat != "no" && path.Trim(null).Length == 0)
                    throw new InputValueException(path, "\"{0}\" is not a valid path.", path);
                spinUpClimateFile = value;
            }
        }
        //---------------------------------------------------------------------
        public bool FireClimate
        {
            get
            {
                return fireClimate;
            }
            set
            {

                fireClimate = value;
            }
        }

        public double RHSlopeAdjust
        {
            get
            {
                return rHSlopeAdjust;
            }
            set
            {

                rHSlopeAdjust = value;
            }
        }

        public int SpringStart
        {
            get
            {
                return springStart;
            }
            set
            {
                springStart = value;
            }
        }

        public int WinterStart
        {
            get
            {
                return winterStart;
            }
            set
            {
                winterStart = value;
            }
        }

    }
}
