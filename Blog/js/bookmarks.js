//
// DotNetNuke -  http://www.dotnetnuke.com
// Copyright (c) 2002-2010
// by Perpetual Motion Interactive Systems Inc. ( http://www.perpetualmotion.ca )
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
//-------------------------------------------------------------------------

function ShareBadge(inputId) {
	//if(!inputId)inputId = this.id;
	//alert(inputId);
	var numericId = this.id.replace(/[^0-9]/g,'');
	var blogInfo = document.getElementById('blogInfo_' + numericId);
	var arrBlogInfo = blogInfo.innerHTML.split("|");

	switch(String(this.getAttribute('rel').toLowerCase())) {
		case 'delicious':
	  		window.open('http://del.icio.us/post?url='+escape(arrBlogInfo[1])+'&title='+escape(arrBlogInfo[0]));
	  		break    
		case 'digg':
	  		window.open('http://digg.com/submit?phase=2&url='+escape(arrBlogInfo[1])+'&title='+escape(arrBlogInfo[0])+'&bodytext='+arrBlogInfo[2]+'&topic=tech_news');
	  		break
		case 'furl':
	  		window.open('http://furl.net/storeIt.jsp?u='+escape(arrBlogInfo[1])+'&t='+escape(arrBlogInfo[0]));
	  		break
		case 'technorati':
	  		window.open('http://www.technorati.com/faves?add='+escape(arrBlogInfo[1])+'');
	  		break
		case 'reddit':
	  		window.open('http://reddit.com/submit?url='+escape(arrBlogInfo[1])+'&title='+escape(arrBlogInfo[0]));
	  		break
		case 'stumble':
	  		window.open('http://www.stumbleupon.com/submit?url='+escape(arrBlogInfo[1])+'&title='+escape(arrBlogInfo[0]));
	  		break
		case 'magnolia':
	  		window.open('http://ma.gnolia.com/bookmarklet/add?url='+escape(arrBlogInfo[1])+'&title='+escape(arrBlogInfo[0]));
	  		break
		case 'blogmarks':
	  		window.open('http://blogmarks.net/my/new.php?mini=1&url='+escape(arrBlogInfo[1])+'&title='+escape(arrBlogInfo[0]));
	  		break
		case 'newsvine':
	  		window.open('http://www.newsvine.com/_wine/save?u='+escape(arrBlogInfo[1])+'&h='+escape(arrBlogInfo[0]));
	  		break
		case 'blinklist':
	  		window.open('http://blinklist.com/index.php?Action=Blink/addblink.php&Url='+arrBlogInfo[1]+'&Title='+escape(arrBlogInfo[0]));
	  		break
		case 'google':
	  		window.open('http://www.google.com/bookmarks/mark?op=edit&bkmk='+arrBlogInfo[1]+'&title='+escape(arrBlogInfo[0]));
	  		break
		case 'yahoo':
	  		window.open('http://myweb2.search.yahoo.com/myresults/bookmarklet?u='+arrBlogInfo[1]+'&t='+escape(arrBlogInfo[0]));
	  		break
		case 'live':
	  		window.open('https://favorites.live.com/quickadd.aspx?marklet=1&mkt=en-us&url='+arrBlogInfo[1]+'&title='+escape(arrBlogInfo[0]));
	  		break
		case 'netscape':
	  		window.open('http://www.netscape.com/submit/?U='+arrBlogInfo[1]+'&T='+escape(arrBlogInfo[0]));
	  		break
		case 'tailrank':
	  		window.open('http://tailrank.com/share/?link_href='+arrBlogInfo[1]+'&title='+escape(arrBlogInfo[0]));
	  		break
		case 'rojo':
	  		window.open('http://www.rojo.com/login/?landing=/submit/?url='+arrBlogInfo[1]+'&ready=true&title='+escape(arrBlogInfo[0]));
	  		break
		case 'icomments':
	  		window.open('http://co.mments.com/track?url='+arrBlogInfo[1]+'&title='+escape(arrBlogInfo[0]));
	  		break
		case 'spurl':
	  		window.open('http://www.spurl.net/spurl.php?url='+arrBlogInfo[1]+'&title='+escape(arrBlogInfo[0]));
	  		break
		case 'delirious':
	  		window.open('http://de.lirio.us/rubric/post?uri='+arrBlogInfo[1]+';when_done=go_back;title='+escape(arrBlogInfo[0]));
	  		break
		case 'linkagogo':
	  		window.open('http://www.linkagogo.com/go/AddNoPopup?url='+arrBlogInfo[1]+'&title='+escape(arrBlogInfo[0]));
	  		break
		case 'pluck':
	  		window.open('http://client.pluck.com/pluckit/prompt.aspx?a='+arrBlogInfo[1]+'&title='+escape(arrBlogInfo[0]));
	  		break
		case 'blogbuzz':
	  		window.open('http://www.blogg-buzz.com/submit.php?url='+arrBlogInfo[1]+'&title='+escape(arrBlogInfo[0]));
	  		break
		case 'fark':
	  		window.open('http://cgi.fark.com/cgi/fark/submit.pl?new_url='+arrBlogInfo[1]+'&new_comment='+escape(arrBlogInfo[0]));
	  		break
		case 'shadows':
	  		window.open('http://www.shadows.com/bookmark/saveLink.rails?page='+arrBlogInfo[1]+'&title='+escape(arrBlogInfo[0]));
			break
		case 'jeeves':
	  		window.open('http://myjeeves.ask.com/mysearch/BookmarkIt?v=1.2&t=webpages&url='+arrBlogInfo[1]+'&title='+escape(arrBlogInfo[0]));
	  		break
		case 'simpy':
	  		window.open('http://www.simpy.com/simpy/LinkAdd.do?href='+arrBlogInfo[1]+'&title='+escape(arrBlogInfo[0]));
	  		break
		case 'rawsugar':
	  		window.open('http://www.rawsugar.com/tagger/?turl='+arrBlogInfo[1]+'&tttl='+escape(arrBlogInfo[0])+'&editorInitialized=1');
	  		break
		case 'netvouz':
	  		window.open('http://netvouz.com/action/submitBookmark?url='+arrBlogInfo[1]+'&title='+escape(arrBlogInfo[0]));
	  		break
		case 'bluedot':
	  		window.open('http://bluedot.us/Authoring.aspx?u='+arrBlogInfo[1]+'&t='+escape(arrBlogInfo[0]));
	  		break
		case 'feedmelinks':
	  		window.open('http://feedmelinks.com/categorize?from=toolbar&op=submit&url='+arrBlogInfo[1]+'&name='+escape(arrBlogInfo[0]));
	  		break
		case 'wink':
	  		window.open('http://wink.com/_/tag?url='+arrBlogInfo[1]+'&doctitle='+escape(arrBlogInfo[0]));
	  		break
		case 'backflip':
	  		window.open('http://www.backflip.com/add_page_pop.ihtml?url='+arrBlogInfo[1]+'&title='+escape(arrBlogInfo[0]));
	  		break
		case 'addfav':
			addFav(arrBlogInfo[1],arrBlogInfo[0]);
	  		break
		default:
		// NOOP
		} 
}

function addFav(url, title) {
	if(window.sidebar) { 
		window.sidebar.addPanel(title, url, ""); 
	} else if(window.external) { 
		window.external.AddFavorite(url, title); 
	} else 
	alert('Sorry, ShareBadge does not currently support adding favorites on your current browser.');
}		

var ShareBadge_slideSpeed = 10;	// Higher value = faster
var ShareBadge_timer = 10;	// Lower value = faster

var objectIdToSlideDown = false;
var ShareBadge_bctiveId = false;
var ShareBadge_slideInProgress = false;

function showHideContent(e,inputId)
{
	if(ShareBadge_slideInProgress)return;
	ShareBadge_slideInProgress = true;
	if(!inputId)inputId = this.id;
	inputId = inputId + '';
	var numericId = inputId.replace(/[^0-9]/g,'');
	
	var answerDiv = document.getElementById('ShareBadge_b' + numericId);

	objectIdToSlideDown = false;
	
	if(!answerDiv.style.display || answerDiv.style.display=='none'){		
		if(ShareBadge_bctiveId &&  ShareBadge_bctiveId!=numericId){			
			objectIdToSlideDown = numericId;
			slideContent(ShareBadge_bctiveId,(ShareBadge_slideSpeed*-1));
		}else{
			
			answerDiv.style.display='block';
			answerDiv.style.visibility = 'visible';
			
			slideContent(numericId,ShareBadge_slideSpeed);
		}
	}else{
		slideContent(numericId,(ShareBadge_slideSpeed*-1));
		ShareBadge_bctiveId = false;
	}	
}

function slideContent(inputId,direction)
{
	
	var obj =document.getElementById('ShareBadge_b' + inputId);
	var contentObj = document.getElementById('ShareBadge_bc' + inputId);
	height = obj.clientHeight;
	if(height==0)height = obj.offsetHeight;
	height = height + direction;
	rerunFunction = true;
	if(height>contentObj.offsetHeight){
		height = contentObj.offsetHeight;
		rerunFunction = false;
	}
	if(height<=1){
		height = 1;
		rerunFunction = false;
	}

	obj.style.height = height + 'px';
	var topPos = height - contentObj.offsetHeight;
	if(topPos>0)topPos=0;
	contentObj.style.top = topPos + 'px';
	if(rerunFunction){
		setTimeout('slideContent(' + inputId + ',' + direction + ')',ShareBadge_timer);
	}else{
		if(height<=1){
			obj.style.display='none'; 
			if(objectIdToSlideDown && objectIdToSlideDown!=inputId){
				document.getElementById('ShareBadge_b' + objectIdToSlideDown).style.display='block';
				document.getElementById('ShareBadge_b' + objectIdToSlideDown).style.visibility='visible';
				slideContent(objectIdToSlideDown,ShareBadge_slideSpeed);				
			}else{
				ShareBadge_slideInProgress = false;
			}
		}else{
			ShareBadge_bctiveId = inputId;
			ShareBadge_slideInProgress = false;
		}
	}
}
function initShareBadge()
{
	var divs = document.getElementsByTagName('DIV');

	var divCounter = 1;
	for(var no=0;no<divs.length;no++){
		if(divs[no].className=='ShareBadge_Link'){
			divs[no].onclick = showHideContent;
			divs[no].id = 'ShareBadge_l'+divCounter;
			var answer = divs[no].nextSibling;
			while(answer && answer.tagName!='DIV'){
				answer = answer.nextSibling;
			}
			answer.id = 'ShareBadge_b'+divCounter;	
			contentDiv = answer.getElementsByTagName('DIV')[0];
			contentDiv.style.top = 0 - contentDiv.offsetHeight + 'px'; 	
			contentDiv.className='ShareBadge_Box_content';
			contentDiv.id = 'ShareBadge_bc' + divCounter;
			answer.style.display='none';
			answer.style.height='1px';
			
			
			var blogInfo = contentDiv.nextSibling;
			while(blogInfo && blogInfo.tagName!='DIV'){
				blogInfo = blogInfo.nextSibling;
			}
			blogInfo.id = 'blogInfo_'+divCounter;
			
			var icons = answer.getElementsByTagName('IMG');
			for(var iconno=0;iconno<icons.length;iconno++){
				
				if(icons[iconno].rel != 'undefined' || icons[iconno].rel != '') {
					icons[iconno].onclick=ShareBadge;
					icons[iconno].onmouseover=DisplayBadgeInfo;
					icons[iconno].onmouseout=HideBadgeInfo;
					icons[iconno].id='who_cares' + '_' + divCounter;
					icons[iconno].className='iconLink';
					//alert(icons(iconno).id);
				}
			}
			divCounter++;
		}		
	}	
}

function DisplayBadgeInfo(inputId) { 
  status=this.alt;
  return false;
}

function HideBadgeInfo() { 
  status='';
   return false;
}