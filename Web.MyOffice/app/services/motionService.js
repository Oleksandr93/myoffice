﻿(function () {
    'use strict';

    angular.module('MyOffice.app').service('motionService', ['$resource', motionService]);

    function motionService($resource) {
        return $resource('api/motions/', {}, {
            motionList: { method: 'GET', params: {}, isArray: true },
            motionDelete: { url: 'api/motions/delete', method: 'POST', params: {}, isArray: false },
            motionUpdate: { method: 'PUT', parmas: {}, isArray: false },
            motionMerge: { method: 'POST', parmas: {}, isArray: false }
        });
    }
})();
