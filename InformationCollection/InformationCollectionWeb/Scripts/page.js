
/*
showPages v1.1
=================================

Infomation
----------------------
Author : Lapuasi
E-Mail : lapuasi@gmail.com
Web : http://www.lapuasi.com
Date : 2005-11-17

Example
----------------------
var pg = new showPages('pg');
pg.pageCount = 12; 
pg.argName = 'p';  
pg.printHtml();   

Supported in Internet Explorer, Mozilla Firefox
*/

function showPages(name) { 
	this.name = name;
	this.mode = 0;
	this.page = 1;         
	this.pageCount = 1;    
	this.argName = 'page';
	this.showTimes = 1;
	this.callbackFunc = null;
	this.isAjax = false;
}

showPages.prototype.goToFunc = function (Num) {
    if (this.callbackFunc != null)
        this.callbackFunc(Num);
}
showPages.prototype.getPage = function(Num){ 
	this.page = Num;
}
showPages.prototype.checkPages = function(){ 
	if (isNaN(parseInt(this.page))) this.page = 1;
	if (isNaN(parseInt(this.pageCount))) this.pageCount = 1;
	if (this.page < 1) this.page = 1;
	if (this.pageCount < 1) this.pageCount = 1;
	if (this.page > this.pageCount) this.page = this.pageCount;
	this.page = parseInt(this.page);
	this.pageCount = parseInt(this.pageCount);
}
showPages.prototype.createHtml = function(mode){
	var strHtml = '', prevPage = this.page - 1, nextPage = this.page + 1;
	if (mode == '' || typeof(mode) == 'undefined') mode = 0;
	switch (mode) {
		case 0 : 
			strHtml += '<span class="count">Pages: ' + this.page + ' / ' + this.pageCount + '</span>';
			strHtml += '<span class="number">';
			if (prevPage < 1) {
				strHtml += '<span title="First Page">首页</span>';
				strHtml += '<span title="Prev Page">上一页</span>';
			} else {
			    strHtml += '<span title="First Page"><a href="" onclick="return ' + this.name + '.toPage(1);">首页</a></span>';
				strHtml += '<span title="Prev Page"><a href="" onclick="return ' + this.name + '.toPage(' + prevPage + ');">上一页</a></span>';
			}
			for (var i = 1; i <= this.pageCount; i++) {
				if (i > 0) {
					if (i == this.page) {
						strHtml += '<span title="Page ' + i + '">[' + i + ']</span>';
					} else {
						strHtml += '<span title="Page ' + i + '"><a href="" onclick="return ' + this.name + '.toPage(' + i + ');">[' + i + ']</a></span>';
					}
				}
			}
			if (nextPage > this.pageCount) {
				strHtml += '<span title="Next Page">下一页</span>';
				strHtml += '<span title="Last Page">尾页</span>';
			} else {
			    strHtml += '<span title="Next Page"><a href="" onclick="return ' + this.name + '.toPage(' + nextPage + ');">下一页</a></span>';
				strHtml += '<span title="Last Page"><a href="" onclick="return ' + this.name + '.toPage(' + this.pageCount + ');">尾页</a></span>';
			}
			strHtml += '</span><br />';
			break;
		case 1 : 
			strHtml += '<span class="count">Pages: ' + this.page + ' / ' + this.pageCount + '</span>';
			strHtml += '<span class="number">';
			if (prevPage < 1) {
				strHtml += '<span title="First Page">首页</span>';
				strHtml += '<span title="Prev Page">上一页</span>';
			} else {
				strHtml += '<span title="First Page"><a href="" onclick="return ' + this.name + '.toPage(1);">首页</a></span>';
				strHtml += '<span title="Prev Page"><a href="" onclick="return ' + this.name + '.toPage(' + prevPage + ');">上一页</a></span>';
			}
			if (this.page % 5 ==0) {
				var startPage = this.page - 4;
			} else {
				var startPage = this.page - this.page % 5 + 1;
			}
			if (startPage > 5) strHtml += '<span title="Prev 10 Pages"><a href="" onclick="return ' + this.name + '.toPage(' + (startPage - 1) + ');">...</a></span>';
			for (var i = startPage; i < startPage + 5; i++) {
				if (i > this.pageCount) break;
				if (i == this.page) {
					strHtml += '<span title="Page ' + i + '">[' + i + ']</span>';
				} else {
					strHtml += '<span title="Page ' + i + '"><a href="" onclick="return ' + this.name + '.toPage(' + i + ');">[' + i + ']</a></span>';
				}
			}
			if (this.pageCount >= startPage + 5) strHtml += '<span title="Next 10 Pages"><a href="" onclick="return ' + this.name + '.toPage(' + (startPage + 5) + ');">...</a></span>';
			if (nextPage > this.pageCount) {
				strHtml += '<span title="Next Page">下一页</span>';
				strHtml += '<span title="Last Page">尾页</span>';
			} else {
				strHtml += '<span title="Next Page"><a href="" onclick="return ' + this.name + '.toPage(' + nextPage + ');">下一页</a></span>';
				strHtml += '<span title="Last Page"><a href="" onclick="return ' + this.name + '.toPage(' + this.pageCount + ');">尾页</a></span>';
			}
			strHtml += '</span><br />';
			break;
		case 2 : 
			strHtml += '<span class="count">Pages: ' + this.page + ' / ' + this.pageCount + '</span>';
			strHtml += '<span class="number">';
			if (prevPage < 1) {
				strHtml += '<span title="First Page">首页</span>';
				strHtml += '<span title="Prev Page">上一页</span>';
			} else {
				strHtml += '<span title="First Page"><a href="" onclick="return ' + this.name + '.toPage(1);">首页</a></span>';
				strHtml += '<span title="Prev Page"><a href="" onclick="return ' + this.name + '.toPage(' + prevPage + ');">上一页</a></span>';
			}
			if (this.page != 1) strHtml += '<span title="Page 1"><a href="" onclick="return ' + this.name + '.toPage(1);">[1]</a></span>';
			if (this.page >= 4) strHtml += '<span>...</span>';
			if (this.pageCount > this.page + 1) {
				var endPage = this.page + 1;
			} else {
				var endPage = this.pageCount;
			}
			for (var i = this.page - 1; i <= endPage; i++) {
				if (i > 0) {
					if (i == this.page) {
						strHtml += '<span title="Page ' + i + '">[' + i + ']</span>';
					} else {
						if (i != 1 && i != this.pageCount) {
							strHtml += '<span title="Page ' + i + '"><a href="" onclick="return ' + this.name + '.toPage(' + i + ');">[' + i + ']</a></span>';
						}
					}
				}
			}
			if (this.page + 2 < this.pageCount) strHtml += '<span>...</span>';
			if (this.page != this.pageCount) strHtml += '<span title="Page ' + this.pageCount + '"><a href="" onclick="return ' + this.name + '.toPage(' + this.pageCount + ');">[' + this.pageCount + ']</a></span>';
			if (nextPage > this.pageCount) {
				strHtml += '<span title="Next Page">下一页</span>';
				strHtml += '<span title="Last Page">尾页</span>';
			} else {
				strHtml += '<span title="Next Page"><a href="" onclick="return ' + this.name + '.toPage(' + nextPage + ');">下一页</a></span>';
				strHtml += '<span title="Last Page"><a href="" onclick="return ' + this.name + '.toPage(' + this.pageCount + ');">尾页</a></span>';
			}
			strHtml += '</span><br />';
			break;
		case 3 : 
			if (this.pageCount < 1) {
				strHtml += '<select name="toPage" disabled>';
				strHtml += '<option value="0">No Pages</option>';
			} else {
				var chkSelect;
				strHtml += '<select name="toPage" onchange="' + this.name + '.toPage(this);">';
				for (var i = 1; i <= this.pageCount; i++) {
					if (this.page == i) chkSelect=' selected="selected"';
					else chkSelect='';
					strHtml += '<option value="' + i + '"' + chkSelect + '>Pages: ' + i + ' / ' + this.pageCount + '</option>';
				}
			}
			strHtml += '</select>';
			break;
		case 4:
            strHtml += '<span class="number">';
            if (prevPage < 1) {
                strHtml += '<span title="First Page">&lt;&lt;</span>&nbsp;';
                strHtml += '<span title="Prev Page">&lt;Previous</span>';
            } else {
                strHtml += '<span title="First Page"><a href="" onclick="return ' + this.name + '.toPage(1);">&lt;&lt;</a></span>&nbsp;';
                strHtml += '<span title="Prev Page"><a href="" onclick="return ' + this.name + '.toPage(' + prevPage + ');">&lt;Previous</a></span>';
            }
            strHtml += '</span>';
            strHtml += '<span class="input">';
            if (this.pageCount < 1) {
                strHtml += '<input type="text" name="toPage" value="No Pages" class="itext" disabled="disabled">';
                strHtml += '<input type="button" name="go" value="GO" class="ibutton" disabled="disabled"></option>';
            } else {
                strHtml += '<input type="text" value="Input Page:" class="ititle" readonly="readonly">';
                strHtml += '<input type="text" id="pageInput' + this.showTimes + '" value="' + this.page + '" class="itext" title="Input page" onkeypress="return ' + this.name + '.formatInputPage(event);" onfocus="this.select()">';
                strHtml += '<input type="text" value=" / ' + this.pageCount + '" class="icount" readonly="readonly">';
                strHtml += '<input type="button" name="go" value="GO" class="ibutton" onclick="return ' + this.name + '.toPage(document.getElementById(\'pageInput' + this.showTimes + '\').value);"></option>';
            }
            strHtml += '</span>';
            strHtml += '<span class="number">';
            if (nextPage > this.pageCount) {
                strHtml += '<span title="Next Page">Next&gt;</span>';
                strHtml += '<span title="Last Page">&gt;&gt;</span>';
            } else {
                strHtml += '<span title="Next Page"><a href="" onclick="return ' + this.name + '.toPage(' + nextPage + ');">Next&gt;</a></span>';
                strHtml += '<span title="Last Page"><a href="" onclick="return ' + this.name + '.toPage(' + this.pageCount + ');">&gt;&gt;</a></span>';
            }
            strHtml += '</span>';
            break;
		case 5 : 
			strHtml += '<span class="number">';
			if (prevPage < 1) {
				strHtml += '<span title="First Page">&lt;&lt;</span>&nbsp;';
				strHtml += '<span title="Prev Page">&lt;Previous</span>';
			} else {
				strHtml += '<span title="First Page"><a href="" onclick="return ' + this.name + '.toPage(1);">&lt;&lt;</a></span>&nbsp;';
				strHtml += '<span title="Prev Page"><a href="" onclick="return ' + this.name + '.toPage(' + prevPage + ');">&lt;Previous</a></span>';
			}
			strHtml += ' | Pages:';
			if (this.page != 1) strHtml += '<span title="Page 1"><a href="" onclick="return ' + this.name + '.toPage(1);">[1]</a></span>';
			if (this.page >= 4) strHtml += '<span>...</span>';
			if (this.pageCount > this.page + 1) {
				var endPage = this.page + 1;
			} else {
				var endPage = this.pageCount;
			}
			for (var i = this.page - 1; i <= endPage; i++) {
				if (i > 0) {
					if (i == this.page) {
						strHtml += '<span title="Page ' + i + '">[' + i + ']</span>';
					} else {
						if (i != 1 && i != this.pageCount) {
							strHtml += '<span title="Page ' + i + '"><a href="" onclick="return ' + this.name + '.toPage(' + i + ');">[' + i + ']</a></span>';
						}
					}
				}
			}
			if (this.page + 2 < this.pageCount) strHtml += '<span>...</span>';
			if (this.page != this.pageCount) strHtml += '<span title="Page ' + this.pageCount + '"><a href="" onclick="return ' + this.name + '.toPage(' + this.pageCount + ');">[' + this.pageCount + ']</a></span>';
			strHtml += ' | ';
			if (nextPage > this.pageCount) {
				strHtml += '<span title="Next Page">Next&gt;</span>';
				strHtml += '<span title="Last Page">&gt;&gt;</span>';
			} else {
				strHtml += '<span title="Next Page"><a href="" onclick="return ' + this.name + '.toPage(' + nextPage + ');">Next&gt;</a></span>';
				strHtml += '<span title="Last Page"><a href="" onclick="return ' + this.name + '.toPage(' + this.pageCount + ');">&gt;&gt;</a></span>';
			}
			strHtml += ' | ';
			if(this.page==0)
				strHtml += '<span title="View All">View All</span>';
			else
				strHtml += '<span title="View All"><a href="" onclick="return ' + this.name + '.toPage(0);">View All</a></span>';
			strHtml += '</span><br />';
			break;
        case 6:
            strHtml += '<span class="number">';
            if (prevPage < 1) {
                strHtml += '<span title="First Page">&lt;&lt;</span>&nbsp;';
                strHtml += '<span title="Prev Page">&lt;Previous</span>';
            } else {
                strHtml += '<span title="First Page"><a href="" onclick="return ' + this.name + '.toPage(1);">&lt;&lt;</a></span>&nbsp;';
                strHtml += '<span title="Prev Page"><a href="" onclick="return ' + this.name + '.toPage(' + prevPage + ');">&lt;Previous</a></span>';
            }
            strHtml += ' | Pages:';
            if (this.page != 1) strHtml += '<span title="Page 1"><a href="" onclick="return ' + this.name + '.toPage(1);">[1]</a></span>';
            if (this.page >= 4) strHtml += '<span>...</span>';
            if (this.pageCount > this.page + 1) {
                var endPage = this.page + 1;
            } else {
                var endPage = this.pageCount;
            }
            for (var i = this.page - 1; i <= endPage; i++) {
                if (i > 0) {
                    if (i == this.page) {
                        strHtml += '<span title="Page ' + i + '">[' + i + ']</span>';
                    } else {
                        if (i != 1 && i != this.pageCount) {
                            strHtml += '<span title="Page ' + i + '"><a href="" onclick="return ' + this.name + '.toPage(' + i + ');">[' + i + ']</a></span>';
                        }
                    }
                }
            }
            if (this.page + 2 < this.pageCount) strHtml += '<span>...</span>';
            if (this.page != this.pageCount) strHtml += '<span title="Page ' + this.pageCount + '"><a href="" onclick="return ' + this.name + '.toPage(' + this.pageCount + ');">[' + this.pageCount + ']</a></span>';
            strHtml += ' | ';
            if (nextPage > this.pageCount) {
                strHtml += '<span title="Next Page">Next&gt;</span>';
                strHtml += '<span title="Last Page">&gt;&gt;</span>';
            } else {
                strHtml += '<span title="Next Page"><a href="" onclick="return ' + this.name + '.toPage(' + nextPage + ');">Next&gt;</a></span>';
                strHtml += '<span title="Last Page"><a href="" onclick="return ' + this.name + '.toPage(' + this.pageCount + ');">&gt;&gt;</a></span>';
            }
            strHtml += '</span><br />';
            break;
		case 7:
            strHtml += '<span class="input">';
            if (this.pageCount < 1) {
                strHtml += '<input type="text" name="toPage" value="No Pages" class="itext" disabled="disabled">';
                strHtml += '<input type="button" name="go" value="GO" class="ibutton" disabled="disabled"></option>';
            } else {
                strHtml += '<input type="text" value="Input Page:" class="ititle" readonly="readonly">';
                strHtml += '<input type="text" id="pageInput' + this.showTimes + '" value="' + this.page + '" class="itext" title="Input page" onkeypress="return ' + this.name + '.formatInputPage(event);" onfocus="this.select()">';
                strHtml += '<input type="text" value=" / ' + this.pageCount + '" class="icount" readonly="readonly">';
                strHtml += '<input type="button" name="go" value="GO" class="ibutton" onclick="return ' + this.name + '.toPage(document.getElementById(\'pageInput' + this.showTimes + '\').value);"></option>';
            }
            strHtml += '</span>';
            break;
		default :
			strHtml = 'Javascript showPage Error: not find mode ' + mode;
			break;
	}
	return strHtml;
}

showPages.prototype.toPage = function (page) {
    if (page > this.pageCount)
        page = this.pageCount;
    if (page < 1)
        page = 1;
    this.goToFunc(page);
    if (this.isAjax) {
        this.getPage(page);
        document.getElementById('pages_' + this.name + '_' + this.showTimes).innerHTML = this.createHtml(this.mode);
    }
    return false;
}

showPages.prototype.changePageCount = function(MaxNum,page){
	this.getPage(page);
	this.pageCount = MaxNum;
	this.checkPages();
	document.getElementById('pages_' + this.name + '_' + this.showTimes).innerHTML = this.createHtml(this.mode);
}

showPages.prototype.returnBoxHtml = function(){ 
	this.showTimes += 1;
	return '<span id="pages_' + this.name + '_' + this.showTimes + '" class="pages"></span>';
}

showPages.prototype.printHtml = function (mode) {
    if (isNaN(this.page) || this.page < 1)
        this.getPage(1);
    this.checkPages();
    this.mode = mode;
    var box = document.getElementById('pages_' + this.name + '_' + this.showTimes);
    if (!box) {
        this.showTimes += 1;
        document.write('<span id="pages_' + this.name + '_' + this.showTimes + '" class="pages"></span>');
    }
    document.getElementById('pages_' + this.name + '_' + this.showTimes).innerHTML = this.createHtml(mode);
}

showPages.prototype.formatInputPage = function(e){ 
	var ie = navigator.appName=="Microsoft Internet Explorer"?true:false;
	if(!ie) var key = e.which;
	else var key = event.keyCode;
	if (key == 8 || key == 46 || (key >= 48 && key <= 57)) return true;
	return false;
}

if (typeof (Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();