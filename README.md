# mTouch-PDFReader is a Monotouch library to display PDF documents on iPad. With help of this library you can develop complete applications for displaying PDF documents that includes e-book readers, document browsers, etc. The library is written in Monotouch with use of Cocoa Touch classes for reading and displaying of the PDF documents, without the use of third-party libraries! mTouch-PDFReader can be easily connect to the project and set up. The library is delivered with a demo application that features a majority of the library's functions and with proper documentation.

# mTouch-PDFReader official site: http://mTouch-PDFReader.com

[![mTouch-PDFReader](http://mTouch-PDFReader.com/content/images/logo-vert-big.png)](http://mTouch-PDFReader.com)

Features
--------
* Two options of page turning
  Horizontal
  Vertical
* Autofit pages
  Based on the device's size
  When rotating the device
* Quality page display
  Background page display before the main page is finalized and displayed
* Detailed page scaling
  Page scaling within the limits from 0.5x to 10x
  Scaling by stretching, double click, buttons
* Bookmarks
  Bookmark management View
  Making, deleting bookmarks, jumping to a page by clicking on a bookmark
  Universal bookmark manager 
* Notes
  Notes editing View
  Universal note manager
* Extended navigation
  Buttons to jump one page back and forth, to the first and to the last page, to the selected page
  Navigation through pages with a slider
* Pages thumbnails
  Jumping to a page by clicking on a thumbnail
  Highlighting of a current page thumbnail
* Options View
  Choosing of page turning option
  Displaying and hiding of any elements 
  Setting cache and thumbnails size
* Top bar with a document control buttons
* Bottom bar with a slider for page navigation 
* Fast connection of the library to the project 
* Displaying of large documents, over 100Mb
* Page cache for fast displaying
* Option to localize all string resources 
* Full configuring of the library's behavior and appearance
* No third-party code, only C#, Monotouch and Cocoa Touch


Integration
-----------
To connect the library to the project there are a few simple steps to follow:
1. Add a reference to the library mTouch-PDFReader in the project.
2. Add using mTouch-PDFReader.Library.DataObjects in the module of the AppDelegate class.
3. Activate global objects by loading of the application, in the AppDelegate class:
  public partial class AppDelegate : UIApplicationDelegate
  {
    public override bool FinishedLaunching(UIApplication app, NSDictionary options)
    {
      ... Some initialization code
      ObjectsActivator().CreateObjects(); 
4. Add into your controller module using mTouch-PDFReader.Library.Views.Core.
5. Add the following code into the button handler in the method ViewDidLoaded of your controller:
btnOpenDocument.TouchUpInside += delegate {
     int docId = 1;
     string docName = "My book";
     string docPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "MyBook.pdf");
     var docViewController = new DocumentViewController();
     NavigationController.PushViewController(docViewController, true);
     docViewController.OpenDocument(docId, docName, docPath); 
};

Note: Button btnOpenDocument should be created beforehand, the controller where the action is performed should be inherited from UINavigationViewController.


Licensing
---------
You need a license if you want to distribute this within your application, see License.md for details.  


Screenshots
-----------

![Two options of page turning and autofit pages](http://mTouch-PDFReader.com/content/images/screens/Feature-Turning.png)
Two options of page turning and autofit pages


![Quality page display and detailed page scaling](http://mTouch-PDFReader.com/content/images/screens/Feature-QScale.png)
Quality page display and detailed page scaling


![Bookmarks](http://mTouch-PDFReader.com/content/images/screens/Feature-BookmarksView.png)
Bookmarks


![Notes](http://mTouch-PDFReader.com/content/images/screens/Feature-NoteView.png)
Notes


![Extended navigation](http://mTouch-PDFReader.com/content/images/screens/Feature-ExNavigation.png)
Extended navigation


![Pages thumbnails](http://mTouch-PDFReader.com/content/images/screens/Feature-ThumbsView.png)
Pages thumbnails


![Options View](http://mTouch-PDFReader.com/content/images/screens/Feature-OptionsView.png)
Options View
