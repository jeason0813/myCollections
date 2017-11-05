/*  Prototype JavaScript framework
 *  (c) 2005 Sam Stephenson <sam@conio.net>
 *  Prototype is freely distributable under the terms of an MIT-style license.
 *  For details, see the Prototype web site: http://prototype.conio.net/
/*--------------------------------------------------------------------------*/


var Class = {
	create: function() {
		return function() {
			this.initialize.apply(this, arguments);
		};
	}
};
Object.extend = function(destination, source) {
	for (window.property in source) destination[window.property] = source[window.property];
	return destination;
};
Function.prototype.bind = function(object) {
	var __method = this;
	return function() {
		return __method.apply(object, arguments);
	};
};
Function.prototype.bindAsEventListener = function(object) {
var __method = this;
	return function(event) {
		__method.call(object, event || window.event);
	};
};

function $() {
	if (arguments.length == 1) return window.get$(arguments[0]);
	var elements = [];
	$c(arguments).each(function(el){
		elements.push(window.get$(el));
	});
	return elements;
}

if (!window.Element) var Element = new Object();

Object.extend(Element, {
	
	hasClassName: function(element, className) {
		element = $(element);
		if (!element) return false;
		var hasClass = false;
		element.className.split(' ').each(function(cn){
			if (cn == className) hasClass = true;
		});
		return hasClass;
	}
});

document.getElementsByClassName = function(className) {
	var children = document.getElementsByTagName('*') || document.all;
	var elements = [];
	$c(children).each(function(child){
		if (Element.hasClassName(child, className)) elements.push(child);
	});  
	return elements;
};
//useful array functions
Array.prototype.iterate = function(func){
	for(var i=0;i<this.length;i++) func(this[i], i);
};
if (!Array.prototype.each) Array.prototype.each = Array.prototype.iterate;

function $c(array){
	var nArray = [];
	for (var i=0;i<array.length;i++) nArray.push(array[i]);
	return nArray;
}
