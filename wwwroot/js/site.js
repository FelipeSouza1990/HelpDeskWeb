(() => {
    // ===== Tema (dark/light) =====
    const root = document.documentElement;
    const saved = localStorage.getItem("theme");
    if (saved) root.setAttribute("data-theme", saved);
  
    window.toggleTheme = () => {
      const next = root.getAttribute("data-theme") === "light" ? "" : "light";
      if (next) root.setAttribute("data-theme", next);
      else root.removeAttribute("data-theme");
      localStorage.setItem("theme", next);
    };
  
    // ===== Toast helper =====
    const area = document.querySelector(".toast-area") || (() => {
      const d = document.createElement("div");
      d.className = "toast-area";
      document.body.appendChild(d);
      return d;
    })();
  
    window.toast = (msg) => {
      const el = document.createElement("div");
      el.className = "toast";
      el.textContent = msg;
      area.appendChild(el);
      setTimeout(() => el.remove(), 3500);
    };
  
    // ===== Util: classe do badge por status =====
    function statusBadgeClass(s) {
      switch (s) {
        case "Resolved":    return "badge status-resolved";
        case "InProgress":  return "badge status-inprog";
        default:            return "badge status-open";
      }
    }
  
    // ===== PATCH de status (botões .status-btn na tabela) =====
    document.addEventListener("click", async (e) => {
      const btn = e.target.closest(".status-btn");
      if (!btn) return;
  
      const tr = btn.closest("tr");
      if (!tr) return;
  
      const id = tr.getAttribute("data-id");
      const newStatus = btn.getAttribute("data-status");
  
      btn.disabled = true;
      try {
        const res = await fetch(`/api/tickets/${id}/status`, {
          method: "PATCH",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({ status: newStatus }) // 👈 agora envia o DTO correto
        });
  
        if (!res.ok) {
          const err = await res.json().catch(() => ({}));
          throw new Error(err.error || res.statusText || "Falha ao atualizar status");
        }
  
        const cell = tr.querySelector(".status");
        if (cell) {
          cell.innerHTML = `<span class="${statusBadgeClass(newStatus)}">${newStatus}</span>`;
        }
        if (window.toast) toast(`Ticket #${id} → ${newStatus}`);
      } catch (err) {
        alert(err.message || "Falha ao atualizar status");
      } finally {
        btn.disabled = false;
      }
    });
  })();
  