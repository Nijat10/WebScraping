# WebScraping

In this solution I created 2 projects (Core MVC and Core WebApi)
So I used the link https://search.ipaustralia.gov.au/trademarks/search/advanced.
But when we search for a specific word, this website changes (encrypts) the value of the word I'm searching for. So I set the default encoding of the word (For example, "abc = 86e4a92e-62e6-4715-a925-1cea2105e0ad")
But if there was a keyword, I can encrypt the word we're looking for and send it to the parameter. So by default I sent the equivalent of "abc" to the parameter. I have noted the details in the code I wrote.

I have created 2 endpoints in my API project. First, in the endpoint, we send 2 parameters. (Page number and the word we are looking for)
It gives us information relevant to that page and word.

In the second endpoint, we send the word we are looking for, and it returns the number of the total result.

And I used it to show my MVC project more visually. Here too pagination, total count and all data are shown.

Thanks in advance !