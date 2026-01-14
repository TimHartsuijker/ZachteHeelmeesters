<template>
  <div>
    <h1>Create Referral</h1>
    <form>
      <input v-model="referral.patientName" placeholder="Patient Name" />
      <input v-model="referral.ReferralCode" placeholder="Referral Code" />
      <input v-model="referral.Note" placeholder="Note" />
      <button @click.prevent="submitReferral">Submit</button>
    </form>
  </div>
</template>

<script>
export default {
  data() {
    return {
      referral: {
        patientName: ''
      }
    }
  },
  methods: {
    submitReferral() {
      console.log('Referral submitted:', this.referral)
    }
  }
}
</script>