# Flat file publishing for .Net

TextPub makes it possible to publish Markdown formatted texts in your DropBox account to your web site. When copied, the files are parsed and read into objects which can be used to display the texts. 

As all folders/files in the DropBox app folder is copied, it is also possible to publish images, css/js files, etc. to the web server.

Here is how it works:

1. All files in your DropBox app folder are copied to the web server where TextPub is installed every 60 minutes (by default).
2. Files that reside in the special folders "articles", "pages" and "snippets" (can be changed) are parsed and read into C# objects (`ArticleModel`, `PageModel`, `SnippetModel` respectively). 
3. Once the C# equivalents to the files has been created, they are put into `HttpRuntime.Cache` for fast-as-lightning access.

## Entity types
All entities share the same base class which contains two properties:

* **Id**: "URL-friendly" string generated from the path and name of the file.
* **Path**: The relative path to the file.

### Articles
All files in the `/articles` folder are converted to ArticleModel's which contains the following properties:

* **Title**: If the text starts with a header tag, it's contents will be used as title. If not, the name of the file will be used as title.
* **Body**: The HTML-formatted contents of the file but without the title if any.
* **PublishDate**: By default this is set to the last modified date of the file, but by putting a date within brackets _after_ the title of the article, it can be overridden (an example: `# My article title [2012-01-01]`).
* **Category**: A category model if applicable.

#### Article categories
By creating sub folders to `/articles`, article categories (and sub categories) are made. Each folder will be converted to a `CategoryModel`.

### Pages
All files in the `/pages` folder are converted to PageModel's which contains the following properties:

* **Title**: If the text starts with a header tag, it's contents will be used as title. If not, the name of the file will be used as title.
* **Body**: The HTML-formatted contents of the file but without the title if any.
* **Level**: The level of the page in the page hierarchy.
* **Parent**: The parent page model (if any).
* **Children**: Create a sub-folder with the same name as the page file and put files into that sub folder to create a parent-child hierarchy. Any children will be set to this property.
* **SortOrder**: Can be specified by putting an integer within brackets _after_ the page title (an example: `# My page title [5]`


### Snippets
All files in the `/snippets` folder are converted to SnippetModel's which contains the following properties:

* **Content**: The HTML-formatted contents of the snippet.


## Configuration
TextPub automatically creates a config file in your app root folder called `TextPub.config`. That file holds your DropBox credentials but also makes it possible to configure synchronization interval and which folders should be used for "entity conversion". 