//-------------------------------------------//
// Copyright Inspector IT, Inc.
// Antonio Chagoury
// www.inspectorit.com
// www.cto20.com 
// www.antoniochagoury.com
// antonio@inspectorit.com
//-------------------------------------------//

var sb_chicklets;

function initializeChicklets() {
	if (sb_chicklets != null) {
		
		return;
	} else {

	sb_chicklets = new Array();
	sb_chicklets = new Array (40);
		for (i = 0; i < sb_chicklets.length; ++ i)
	sb_chicklets[i] = new Array (4);
	
	//Ask.com
	sb_chicklets[0][0] = strImagePath + "images/i_ask.png";
	sb_chicklets[0][1] = "Ask.com";
	sb_chicklets[0][2] = "jeeves";
	sb_chicklets[0][3] = "http://myjeeves.ask.com/mysearch/BookmarkIt?v=1.2&t=webpages&url={url}&title={title}";
	//Backflip
	sb_chicklets[1][0] = strImagePath + "images/i_backflip.png";
	sb_chicklets[1][1] = "Backflip";
	sb_chicklets[1][2] = "backflip";
	sb_chicklets[1][3] = "http://www.backflip.com/add_page_pop.ihtml?url={url}&title={title}";
	//Blinklist
	sb_chicklets[2][0] = strImagePath + "images/i_blinklist.gif";
	sb_chicklets[2][1] = "Blinklist";
	sb_chicklets[2][2] = "blinklist";
	sb_chicklets[2][3] = "http://blinklist.com/index.php?Action=Blink/addblink.php&Url={url}&Title={title}";
	//Blogbuzz
	sb_chicklets[3][0] = strImagePath + "images/i_blogbuzz.jpg";
	sb_chicklets[3][1] = "Blogbuzz";
	sb_chicklets[3][2] = "blogbuzz";
	sb_chicklets[3][3] = "http://www.blogg-buzz.com/submit.php?url={url}&title={title}";
	//Bluedot
	sb_chicklets[4][0] = strImagePath + "images/i_bluedot.png";
	sb_chicklets[4][1] = "BlueDot";
	sb_chicklets[4][2] = "bluedot";
	sb_chicklets[4][3] = "http://bluedot.us/Authoring.aspx?u={url}&t={title}";
	//Blogmarks
	sb_chicklets[5][0] = strImagePath + "images/i_bm.gif";
	sb_chicklets[5][1] = "BlogMarks";
	sb_chicklets[5][2] = "blogmarks";
	sb_chicklets[5][3] = "http://blogmarks.net/my/new.php?mini=1&url={url}&title={title}";
	//Co_mments
	sb_chicklets[6][0] = strImagePath + "images/i_co_mments.gif";
	sb_chicklets[6][1] = "co_mments";
	sb_chicklets[6][2] = "co_mments";
	sb_chicklets[6][3] = "http://co.mments.com/track?url={url}&title={title}";
	//Delicious
	sb_chicklets[7][0] = strImagePath + "images/i_delicious.gif";
	sb_chicklets[7][1] = "del.icio.us";
	sb_chicklets[7][2] = "del.icio.us";
	sb_chicklets[7][3] = "http://del.icio.us/post?url={url}&title={title}";
	//Delirious
	sb_chicklets[8][0] = strImagePath + "images/i_delirious.png";
	sb_chicklets[8][1] = "del.irio.us";
	sb_chicklets[8][2] = "del.irio.us";
	sb_chicklets[8][3] = "http://de.lirio.us/rubric/post?uri={url};when_done=go_back;title={title}";
	//Digg
	sb_chicklets[9][0] = strImagePath + "images/i_digg.gif";
	sb_chicklets[9][1] = "Digg";
	sb_chicklets[9][2] = "digg";
	sb_chicklets[9][3] = "http://digg.com/submit?phase=2&url={url}&title='{title}&bodytext=&topic=')";
	//Fark
	sb_chicklets[10][0] = strImagePath + "images/i_fark.png";
	sb_chicklets[10][1] = "Fark";
	sb_chicklets[10][2] = "fark";
	sb_chicklets[10][3] = "http://cgi.fark.com/cgi/fark/submit.pl?new_url={url}&new_comment={title}";
	//Feedmelinks
	sb_chicklets[11][0] = strImagePath + "images/i_feedmelinks.png";
	sb_chicklets[11][1] = "Feed Me Links";
	sb_chicklets[11][2] = "feedmelinks";
	sb_chicklets[11][3] = "http://feedmelinks.com/categorize?from=toolbar&op=submit&url={url}&name={title}";
	//Furl
	sb_chicklets[12][0] = strImagePath + "images/i_furl.gif";
	sb_chicklets[12][1] = "Furl";
	sb_chicklets[12][2] = "furl";
	sb_chicklets[12][3] = "http://furl.net/storeIt.jsp?u={url}&t={title}";
	//Google
	sb_chicklets[14][0] = strImagePath + "images/i_google.gif";
	sb_chicklets[14][1] = "Google";
	sb_chicklets[14][2] = "google";
	sb_chicklets[14][3] = "http://www.google.com/bookmarks/mark?op=edit&bkmk={url}&title={title}";
	
	//LinkAGoGo
	sb_chicklets[15][0] = strImagePath + "images/i_linkagogo.png";
	sb_chicklets[15][1] = "LinkAGoGo";
	sb_chicklets[15][2] = "linkagogo";
	sb_chicklets[15][3] = "http://www.linkagogo.com/go/AddNoPopup?url={url}&title={title}";
	
	//Magnolia
	sb_chicklets[16][0] = strImagePath + "images/i_magnolia.gif";
	sb_chicklets[16][1] = "Magnolia";
	sb_chicklets[16][2] = "magnolia";
	sb_chicklets[16][3] = "http://ma.gnolia.com/bookmarklet/add?url={url}&title={title}";
	
	//Live Bookmarks
	sb_chicklets[17][0] = strImagePath + "images/i_live.gif";
	sb_chicklets[17][1] = "Live Bookmarks";
	sb_chicklets[17][2] = "live";
	sb_chicklets[17][3] = "https://favorites.live.com/quickadd.aspx?marklet=0&url={url}&title={title}";
	
	//NetVouz
	sb_chicklets[18][0] = strImagePath + "images/i_netvouz.png";
	sb_chicklets[18][1] = "NetVouz";
	sb_chicklets[18][2] = "netvouz";
	sb_chicklets[18][3] = "http://netvouz.com/action/submitBookmark?url={url}&title{title}";
	
	//Netscape
	sb_chicklets[19][0] = strImagePath + "images/i_netscape.gif";
	sb_chicklets[19][1] = "Netscape";
	sb_chicklets[19][2] = "netscape";
	sb_chicklets[19][3] = "http://www.netscape.com/submit/?U={url}&T={title}";
	
	//Newsvine
	sb_chicklets[20][0] = strImagePath + "images/i_newsvine.gif";
	sb_chicklets[20][1] = "Newsvine";
	sb_chicklets[20][2] = "newsvine";
	sb_chicklets[20][3] = "http://www.newsvine.com/_wine/save?u={url}&h={title}";
	
	//Pluck
	sb_chicklets[21][0] = strImagePath + "images/i_pluck.gif";
	sb_chicklets[21][1] = "Pluck";
	sb_chicklets[21][2] = "pluck";
	sb_chicklets[21][3] = "http://client.pluck.com/pluckit/prompt.aspx?a={url}&title={title}";
	
	//Rawsugar
	sb_chicklets[22][0] = strImagePath + "images/i_rawsugar.png";
	sb_chicklets[22][1] = "Rawsugar";
	sb_chicklets[22][2] = "rawsugar";
	sb_chicklets[22][3] = "http://www.rawsugar.com/tagger/?turl={url}&tttl={title}&editorInitialized=1";
	
	//Reddit
	sb_chicklets[23][0] = strImagePath + "images/i_reddit.gif";
	sb_chicklets[23][1] = "Reddit";
	sb_chicklets[23][2] = "reddit";
	sb_chicklets[23][3] = "http://reddit.com/submit?url={url}&title={title}";
	
	//Rojo
	sb_chicklets[24][0] = strImagePath + "images/i_rojo.gif";
	sb_chicklets[24][1] = "Rojo";
	sb_chicklets[24][2] = "rojo";
	sb_chicklets[24][3] = "http://www.rojo.com/login/?landing=/submit/?url={url}&ready=true&title={title}";
	
	//Shadows
	sb_chicklets[25][0] = strImagePath + "images/i_shadows.png";
	sb_chicklets[25][1] = "Shadows";
	sb_chicklets[25][2] = "shadows";
	sb_chicklets[25][3] = "http://www.shadows.com/bookmark/saveLink.rails?page={url}&title={title}";
	
	//Simpy
	sb_chicklets[26][0] = strImagePath + "images/i_simpy.png";
	sb_chicklets[26][1] = "Simpy";
	sb_chicklets[26][2] = "simpy";
	sb_chicklets[26][3] = "http://www.simpy.com/simpy/LinkAdd.do?href={url}&title={title}";
	
	//Spurl
	sb_chicklets[27][0] = strImagePath + "images/i_spurl.png";
	sb_chicklets[27][1] = "Spurl";
	sb_chicklets[27][2] = "spurl";
	sb_chicklets[27][3] = "http://www.spurl.net/spurl.php?url={url}&title={title}";
	
	//StumbleUpon
	sb_chicklets[27][0] = strImagePath + "images/i_stumble.gif";
	sb_chicklets[27][1] = "StumbleUpon";
	sb_chicklets[27][2] = "stumbleupon";
	sb_chicklets[27][3] = "http://www.stumbleupon.com/submit?url={url}&title={title}";
	
	//Technorati
	sb_chicklets[28][0] = strImagePath + "images/i_technorati.gif";
	sb_chicklets[28][1] = "Technorati";
	sb_chicklets[28][2] = "technorati";
	sb_chicklets[28][3] = "http://www.technorati.com/faves?add={url}";
	
	//TailRank
	sb_chicklets[29][0] = strImagePath + "images/i_tailrank.gif";
	sb_chicklets[29][1] = "TailRank";
	sb_chicklets[29][2] = "tailrank";
	sb_chicklets[29][3] = "http://tailrank.com/share/?link_href={url}&title={title}";
	
	//Wink
	sb_chicklets[30][0] = strImagePath + "images/i_wink.png";
	sb_chicklets[30][1] = "Wink";
	sb_chicklets[30][2] = "wink";
	sb_chicklets[30][3] = "http://wink.com/_/tag?url={url}&doctitle={title}";
		
	//Yahoo
	sb_chicklets[31][0] = strImagePath + "images/i_yahoo.gif";
	sb_chicklets[31][1] = "Yahoo";
	sb_chicklets[31][2] = "yahoo";
	sb_chicklets[31][3] = "http://myweb2.search.yahoo.com/myresults/bookmarklet?u={url}&t={title}";
	
	//DotNetKicks
	sb_chicklets[32][0] = strImagePath + "images/i_dotnetkicks.gif";
	sb_chicklets[32][1] = "DotNetKicks";
	sb_chicklets[32][2] = "dotnetkicks";
	sb_chicklets[32][3] = "http://www.dotnetkicks.com/submit/?url={url}&r=inspectorit&title={title}";
	
	//Facebook
	sb_chicklets[33][0] = strImagePath + "images/i_facebook.gif";
	sb_chicklets[33][1] = "Facebook";
	sb_chicklets[33][2] = "facebook";
	sb_chicklets[33][3] = "http://www.facebook.com/sharer.php?u={url}&t={title}";
	
	//Twitter
	sb_chicklets[34][0] = strImagePath + "images/i_twitter.gif";
	sb_chicklets[34][1] = "Twitter";
	sb_chicklets[34][2] = "twitter";
	sb_chicklets[34][3] = "http://twitter.com/home?status={title}:{url}";
	
	//Diigo
	sb_chicklets[35][0] = strImagePath + "images/i_diigo.gif";
	sb_chicklets[35][1] = "Diigo";
	sb_chicklets[35][2] = "diigo";
	sb_chicklets[35][3] = "http://www.diigo.com/post?url={url}&title={title}";
	}
}