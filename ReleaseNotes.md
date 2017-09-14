### New in 0.3.0 (Released 2017/09/14)
* [Support] Upgrade to Cake v0.22.0
* [Support] Upgrade to .Net 4.6

### New in 0.2.4 (Released 2017/08/23)
* [Feature] Added configuration aliases
* [Feature] Added enable / disable directory browsing

### New in 0.2.3 (Released 2017/08/18)
* [Improvement] Use Cake contrib icon

### New in 0.2.2 (Released 2017/02/03)
* [Improvement] Update Cake.Core reference

### New in 0.2.1 (Released 2016/10/26)
* [Bug] Fixed the SiteApplicationExists return value for local IIS.

### New in 0.2.0 (Released 2016/10/24)
* [Feature] Check application exists

### New in 0.1.9 (Released 2016/10/23)
* [Improvement] Configure enabled protocols
* [Feature] Virtual directory support

### New in 0.1.8 (Released 2016/10/20)
* [Improvement] Fix unit-tests and circular reference in Bindings

### New in 0.1.7 (Released 2016/10/04)
* [Improvement] New binding formats

### New in 0.1.6 (Released 2016/08/29)
* [Improvement] Standardised application authorization / authentication

### New in 0.1.5 (Released 2016/01/16)
* [Bug] Fixed SolutionInfo link

### New in 0.1.4 (Released 2015/12/24)
* [Improvement] Addin xml doc to assembly

### New in 0.1.3 (Released 2015/12/10)
* [Improvement] Add Cake namespace docs
* [Improvement] Update Cake.Core reference
* [Feature] Restart Site / ApplicationPool

### New in 0.1.2 (Released 2015/10/24)
* [Improvement] Remove Cake.Core from nuget

### New in 0.1.1 (Released 2015/09/23)
* [Improvement] Move certificate info to BindingSettings, allowing certificates to be applied at both levels

### New in 0.1.0 (Released 2015/09/21)
* [Improvement] Changed nuget references from dependencies to files

### New in 0.0.9 (Released 2015/09/12)
* [Bug] Fix incorrect log in "GetServer"

### New in 0.0.8 (Released 2015/08/18)
* [Improvement] Site AuthorizationSettings
* [Bug] Add / Remove SiteBindings using wrong settings
* [Improvement] WebFarm tests
* [Improvement] New Choco install script for AppVeyor

### New in 0.0.7 (Released 2015/08/17)
* [Improvement] Path fixes by Jake Scott
* [Improvement] Use GetPhysicalDirectory for ApplicationSettings and WebsiteSettings
* [Feature] WebFarm Aliases

### New in 0.0.6 (Released 2015/08/14)
* [Improvement] AppVeyor install script for IIS and WAS
* [Improvement] Renamed PhysicalPath to PhysicalDirectory in SiteSettings
* [Improvement] Use DirectoryPath instead of string to allow for paths relative to the working directory
* [Improvement] Change manager constructors to include the cake environment
* [Improvement] Add Thread.Sleep exception to start / Stop methods to take into account for IIS delays

### New in 0.0.5 (Released 2015/08/13)
* [Improvement] New UnitTests and Bug fixes by Jake Scott
* [Improvement] AppPool TimeSpan checks
* [Improvement] Added cake build example
* [Improvement] Added troubleshooting info

### New in 0.0.4 (Released 2015/08/10)
* [Improvement] Separate out BindingSettings
* [Feature] Add / Remove site binding
* [Feature] Add / Remove site application
* [Improvement] Change Debug to Information logs

### New in 0.0.3 (Released 2015/08/09)
* [Improvement] ProcessModel settings
* [Improvement] RequestTracing settings
* [Improvement] Shared Web and Ftp create function

### New in 0.0.2 (Released 2015/08/06)
* [Feature] Rewrite by Phillip Sharpe
* [Feature] Add support for remote servers
* [Improvement] Replace mixed functionality deploy methods with focused create / remove methods
* [Feature] Add missing Aliases for sites and pools

### New in 0.0.1 (Released 2015/03/31)
* [Feature] First release by Sergio Silveira.