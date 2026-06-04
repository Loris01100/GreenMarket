window.stripeInterop = {
  stripe: null,
  card: null,

  initialize: function (publishableKey) {
    this.stripe = Stripe(publishableKey);

    const elements = this.stripe.elements();

    this.card = elements.create("card");

    this.card.mount("#card-element");
  },

  confirmPayment: async function (clientSecret) {
    const result = await this.stripe.confirmCardPayment(clientSecret, {
      payment_method: {
        card: this.card,
      },
    });

    if (result.error) {
      return {
        success: false,
        error: result.error.message,
      };
    }

    return {
      success: true,
      paymentIntentId: result.paymentIntent.id,
    };
  },
};
