# Flat file publishing for .Net

TextPub exposes an API to publish Markdown formatted texts from disk. Files that reside in the special folders "posts", "pages" and "snippets" are parsed and transformed into C# objects (`IPost`, `IPage`, `ISnippet` respectively). Once the C# equivalents to the files has been created, they are cached for fast-as-lightning access.

## Entity types
All entities share the same base interface which contains two properties:

* **Id**: "URL-friendly" string generated from the path and name of the file.
* **Path**: The relative path to the file.

### Articles
All files in the `/posts` folder are converted to `IPost`'s which contains the following properties:

* **Title**: If the text starts with a header tag, it's contents will be used as title. If not, the name of the file will be used as title.
* **Body**: The HTML-formatted contents of the file but without the title if any.
* **PublishDate**: By default this is set to the last modified date of the file, but by putting a date within brackets _after_ the title of the article, it can be overridden (an example: `# My post title [2012-01-01]`).
* **Category**: A category model if applicable.

#### Article categories
By creating sub folders to `/posts`, article categories (and sub categories) are made. Each folder will be converted to a `Category`.

### Pages
All files in the `/pages` folder are converted to `IPage`'s which contains the following properties:

* **Title**: If the text starts with a header tag, it's contents will be used as title. If not, the name of the file will be used as title.
* **Body**: The HTML-formatted contents of the file but without the title if any.
* **Level**: The level of the page in the page hierarchy.
* **Parent**: The parent page model (if any).
* **Children**: Create a sub-folder with the same name as the page file and put files into that sub folder to create a parent-child hierarchy. Any children will be set to this property.
* **SortOrder**: Can be specified by putting an integer within brackets _after_ the page title (an example: `# My page title [5]`

### Snippets
All files in the `/snippets` folder are converted to `ISnippet`'s which contains the following properties:

* **Content**: The HTML-formatted contents of the snippet.

## Configuration
By default, TextPub uses app settings in web.config to configure which folders should be used for "entity conversion". Markdown options can be set from code.

## Release notes

### v1.1

- Decoupled Application.Configuration from it's interface (`IConfiguration`). Default configuration implementation is now `WebConfiguration`.
- Added possibility to set markdown parsing options via `Application.Configuration.MarkdownOptions`.

### v1.0.1

- Added DecoratorProvider's.
- Added some tests.
- Bugfixes.

### v1.0

Initial release.