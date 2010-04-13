var sb_toolbar;

function initializeShareBadge(ShareBadgeToolbar, title, url) {
	initializeChicklets();
	sb_toolbar = document.getElementById(ShareBadgeToolbar); 
	sb_toolbar.className = "ShareBadgePRO_Toolbar";
	
	var sb_toolbar_title = document.createElement('div');
	var sb_toolbar_url = document.createElement('div');
	sb_toolbar_url.className = "ShareBadge_hide";
	sb_toolbar_title.className = "ShareBadge_hide";
	sb_toolbar_title.id = ShareBadgeToolbar + "_title";
	sb_toolbar_url.id = ShareBadgeToolbar + "_url";
	sb_toolbar_url.innerText = url;
	sb_toolbar_title.innerText = title;
	
	sb_toolbar.appendChild(sb_toolbar_title);
	sb_toolbar.appendChild(sb_toolbar_url);
}

function addBadgeItem(chickletId) {
  
  	var objImage = new Image;
	objImage.id = 'sb_chickletId_' + chickletId;
	objImage.src = sb_chicklets[chickletId][0];
	objImage.setAttribute('alt', sb_chicklets[chickletId][1]);
	objImage.setAttribute('title', sb_chicklets[chickletId][1]);
	objImage.setAttribute('rel', sb_toolbar.id);
	objImage.className = 'sb_icon';
	objImage.onclick = shareBadge;
	sb_toolbar.appendChild(objImage);
}

function shareBadge(e) {
	var chickletId = this.id.replace(/[^0-9]/g,'');
	var sb_targetToolbarTitle = document.getElementById(this.getAttribute("rel") + '_title');
	var sb_targetToolbarUrl = document.getElementById(this.getAttribute("rel") + '_url');
	
	var actionUrl = sb_chicklets[chickletId][3];
	
	actionUrl = actionUrl.replace('{url}', escape(sb_targetToolbarUrl.innerText));
	actionUrl = actionUrl.replace('{title}', escape(sb_targetToolbarTitle.innerText));
	window.open(actionUrl);
}
