<template>
  <button
    @click.prevent="logout"
    class="bg-green-dark hover:bg-green-darkest hover:cursor-pointer text-white font-bold py-2 px-4 rounded flex items-center gap-2"
  >
    <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
        d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1m0-10V5m-6 6h.01" />
    </svg>
    Uitloggen
  </button>
</template>

<script setup>

async function logout() {
    const token = localStorage.getItem("token");

    const res = await fetch("http://localhost:7240/api/auth/logout", {
      method: "POST",
      headers: {
        Authorization: `Bearer ${token}`,
        "Content-Type": "application/json",
      },
    });

    let data;

    try {
      data = await res.json();
    } catch (e) {
      console.error("Response was not JSON", e);
      return;
    }

    console.log(data);

    console.log(data.message);
    // Redirect naar login
    window.location.href = "/login";
}

</script>