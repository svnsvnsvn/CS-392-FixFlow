/**
 * Persist visible ticket table columns (localStorage), scoped by role/table.
 * Only columns passed in `columns` can be toggled; use required: true for ID/actions (and staff-only fields never listed for residents).
 */
(function () {
    'use strict';

    var STORAGE_KEY = 'fixflow.ticketTableCols.v1';

    function readAll() {
        try {
            var raw = localStorage.getItem(STORAGE_KEY);
            return raw ? JSON.parse(raw) : {};
        } catch (e) {
            return {};
        }
    }

    function writeAll(obj) {
        try {
            localStorage.setItem(STORAGE_KEY, JSON.stringify(obj));
        } catch (e) { /* ignore */ }
    }

    function buildPrefs(scope, columnDefs) {
        var stored = readAll()[scope] || {};
        var prefs = {};
        for (var i = 0; i < columnDefs.length; i++) {
            var c = columnDefs[i];
            if (c.required) {
                prefs[c.key] = true;
                continue;
            }
            if (Object.prototype.hasOwnProperty.call(stored, c.key)) {
                prefs[c.key] = !!stored[c.key];
            } else {
                prefs[c.key] = c.defaultOn !== false;
            }
        }
        return prefs;
    }

    function applyVisibility(table, columnDefs, prefs) {
        if (!table) return;
        for (var i = 0; i < columnDefs.length; i++) {
            var c = columnDefs[i];
            var on = prefs[c.key] !== false;
            var nodes = table.querySelectorAll('[data-tcol="' + c.key + '"]');
            for (var j = 0; j < nodes.length; j++) {
                nodes[j].style.display = on ? '' : 'none';
            }
        }
    }

    function persist(scope, prefs, columnDefs) {
        var out = {};
        for (var i = 0; i < columnDefs.length; i++) {
            var c = columnDefs[i];
            if (!c.required) out[c.key] = !!prefs[c.key];
        }
        var all = readAll();
        all[scope] = out;
        writeAll(all);
    }

    window.initTicketTableColumns = function (options) {
        var table = options.table;
        var scope = options.scope;
        var columnDefs = options.columns || [];
        var panelHost = options.panelHost;
        var toggleBtn = options.toggleBtn;

        if (!table || !scope || !columnDefs.length) return;

        var prefs = buildPrefs(scope, columnDefs);
        applyVisibility(table, columnDefs, prefs);

        if (!panelHost || !toggleBtn) return;

        panelHost.innerHTML = '';
        var optional = columnDefs.filter(function (c) { return !c.required; });
        optional.forEach(function (c) {
            var id = 'tcol-' + scope + '-' + c.key;
            var wrap = document.createElement('label');
            wrap.className = 'tickets-col-option';
            var cb = document.createElement('input');
            cb.type = 'checkbox';
            cb.id = id;
            cb.checked = prefs[c.key] !== false;
            cb.addEventListener('change', function () {
                prefs[c.key] = cb.checked;
                applyVisibility(table, columnDefs, prefs);
                persist(scope, prefs, columnDefs);
            });
            var span = document.createElement('span');
            span.textContent = c.label;
            wrap.appendChild(cb);
            wrap.appendChild(span);
            panelHost.appendChild(wrap);
        });

        toggleBtn.addEventListener('click', function (e) {
            e.stopPropagation();
            var open = panelHost.hidden;
            panelHost.hidden = !open;
            toggleBtn.setAttribute('aria-expanded', open ? 'true' : 'false');
        });

        document.addEventListener('click', function () {
            panelHost.hidden = true;
            toggleBtn.setAttribute('aria-expanded', 'false');
        });
        panelHost.addEventListener('click', function (e) {
            e.stopPropagation();
        });
    };
})();
