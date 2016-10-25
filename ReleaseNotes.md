### New in 0.2.0 (Released 2016/10/24)
* Check application exists

### New in 0.1.9 (Released 2016/10/23)
* Configure enabled protocols
* Virtual directory support

### New in 0.1.8 (Released 2016/10/20)
* Fix unit-tests and circular reference in Bindings

### New in 0.1.7 (Released 2016/10/04)
* New binding formats

### New in 0.1.6 (Released 2016/08/29)
* Standardised application authorization / authentication

### New in 0.1.5 (Released 2016/01/16)
* Fixed SolutionInfo link

### New in 0.1.4 (Released 2015/12/24)
* addin xml doc to assembly

### New in 0.1.3 (Released 2015/12/10)
* Add Cake namespace docs
* Update Cake.Core reference
* Restart Site / ApplicationPool

### New in 0.1.2 (Released 2015/10/24)
* Remove Cake.Core from nuget

### New in 0.1.1 (Released 2015/09/23)
* Move certificate info to BindingSettings, allowing certificates to be applied at both levels

### New in 0.1.0 (Released 2015/09/21)
* Changed nuget references from dependencies to files

### New in 0.0.9 (Released 2015/09/12)
* Fix incorrect log in "GetServer"

### New in 0.0.8 (Released 2015/08/18)
* Site AuthorizationSettings
* Add / Remove SiteBindings using wrong settings
* WebFarm tests
* New Choco install script for AppVeyor

### New in 0.0.7 (Released 2015/08/17)
* Path fixes by Jake Scott
* Use GetPhysicalDirectory for ApplicationSettings and WebsiteSettings
* WebFarm Aliases

### New in 0.0.6 (Released 2015/08/14)
* AppVeyor install script for IIS and WAS
* Renamed PhysicalPath to PhysicalDirectory in SiteSettings
* Use DirectoryPath instead of string to allow for paths relative to the working directory
* Change manager constructors to include the cake environment
* Add Thread.Sleep exception to start / Stop methods to take into account for IIS delays

### New in 0.0.5 (Released 2015/08/13)
* New UnitTests and Bug fixes by Jake Scott
* AppPool TimeSpan checks
* Added cake build example
* Added troubleshooting info

### New in 0.0.4 (Released 2015/08/10)
* Separate out BindingSettings
* Add / Remove site binding
* Add / Remove site application
* Change Debug to Information logs

### New in 0.0.3 (Released 2015/08/09)
* ProcessModel settings
* RequestTracing settings
* Shared Web and Ftp create function

### New in 0.0.2 (Released 2015/08/06)
* Rewrite by Phillip Sharpe
* Add support for remote servers
* Replace mixed functionality deploy methods with focused create / remove methods
* Add missing Aliases for sites and pools

### New in 0.0.1 (Released 2015/03/31)
* First release by Sergio Silveira.