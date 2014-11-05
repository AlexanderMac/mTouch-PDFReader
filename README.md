<img src="https://github.com/AlexanderMac/mTouch-PDFReader/blob/master/Images/header-logo.gif"/>

mTouch-PDFReader is a Xamarin iOS library to display PDF documents on iPad and iPhone. With this library you can develop complete applications for displaying PDF documents that includes e-book readers, document browsers, etc. The library is written on Xamarin iOS with use of Cocoa Touch classes for reading and displaying of the PDF documents, without the use of third-party libraries. mTouch-PDFReader can be easily added to your project.

## Features

* iPad and iPhone support
* Two transition styles:
  * Curl
  * Scroll
* Two navigation orientations:
  * Horizontal
  * Vertical
* Autofit pages:
    * Based on the device's size
    * After rotating the device
* Scaling page:
  * Based on the device's size
  * After rotating the device
* Bookmarks
* Notes
* Pages thumbnails
* Options View
* No third-party code, only C# and Xamarin iOS
* Displaying the large documents (with size < 100Mb)

## How to integrate
To use the library in your project, you should to do a few simple steps:

1. Add a reference to mTouch-PDFReader library in your project.
2. Add *mTouch-PDFReader.Library.DataObjects* namespace into *AppDelegate* class module.
3. Activate global managers in *AppDelegate* class: 
<pre><code>
  public partial class AppDelegate : UIApplicationDelegate {   
    public override bool FinishedLaunching(UIApplication app, NSDictionary options) {     
      ... Some initialization code     
      ObjectsActivator().CreateObjects();
    }
  }
</pre></code>
4. Add *mTouch-PDFReader.Library.Views.Core* namespace into your document browser controller.
5. To open document, add the following code into the button handler:
<pre><code>
  btnOpenDocument.TouchUpInside += delegate {
    int docId = 1;   
    string docName = "My book";   
    string docPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "MyBook.pdf");
    var docViewController = new DocumentViewController();   
    NavigationController.PushViewController(docViewController, true);
    docViewController.OpenDocument(docId, docName, docPath); 
  }
</pre></code>

> Here you go! The opened document will have default settings:)


## Questions and issues
If you have any questions or issues, please write me at: [support@mtouch-pdfreader.com](mailto:support@mtouch-pdfreader.com) or create issue on github.


## Licensing
See License.md for details.  

## Authors

**Alexander Mac** ([amatsibarov@gmail.com](mailto:amatsibarov@gmail.com))

