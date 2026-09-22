window.brokerPortalApi = (() => {
  const baseUrl = window.BROKERPORTAL_API_URL || 'http://127.0.0.1:5082';

  async function request(path, options = {}) {
    const response = await fetch(`${baseUrl}${path}`, {
      ...options,
      headers: { 'Content-Type': 'application/json', ...(options.headers || {}) }
    });
    if (!response.ok) throw new Error(`API request failed: ${response.status}`);
    return response.json();
  }

  return {
    async users(filters = {}) {
      const params = new URLSearchParams(filters);
      return request(`/api/users?${params}`);
    },
    async submissions(filters = {}) {
      const params = new URLSearchParams(filters);
      return request(`/api/commercial/submissions?${params}`);
    },
    async guidelines(filters = {}) {
      const params = new URLSearchParams(filters);
      return request(`/api/underwriting/guidelines?${params}`);
    },
    async createSubmission(payload) {
      return request('/api/commercial/submissions', { method: 'POST', body: JSON.stringify(payload) });
    },
    async health() {
      return request('/health');
    }
  };
})();
