using UnityEngine;
using UnityEngine.Events; // Nécessaire pour utiliser les UnityEvents

public class PlayerHealth : MonoBehaviour 
{
    [Header("Health Settings")]
    [SerializeField] private int _maxHealth = 100;
    private int _currentHealth;

    [Header("Events")]
    public UnityEvent<int> OnHealthChanged; // Événement pour mettre à jour l'UI (envoie la vie actuelle)
    public UnityEvent OnPlayerDied; // Événement déclenché à la mort du joueur

    private void Awake()
    {
        // Au démarrage, le joueur a toute sa vie.
        _currentHealth = _maxHealth;
    }

    private void Start()
    {
        // Informe l'UI de la vie de départ.
        OnHealthChanged?.Invoke(_currentHealth);
    }

    /// <summary>
    /// Inflige des dégâts au joueur.
    /// </summary>
    /// <param name="damageAmount">Le montant des dégâts à infliger.</param>
    public void TakeDamage(int damageAmount)
    {
        if (damageAmount < 0) return; // On ne peut pas infliger de dégâts négatifs.

        // On soustrait les dégâts et on s'assure que la vie ne descend pas en dessous de 0.
        _currentHealth = Mathf.Max(_currentHealth - damageAmount, 0);

        // On déclenche l'événement pour que l'UI ou d'autres systèmes se mettent à jour.
        OnHealthChanged?.Invoke(_currentHealth);

        Debug.Log($"Player took {damageAmount} damage. Current health: {_currentHealth}");

        // Si la vie atteint 0, on déclenche l'événement de mort.
        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Soigne le joueur.
    /// </summary>
    /// <param name="healAmount">Le montant de vie à restaurer.</param>
    public void Heal(int healAmount)
    {
        if (healAmount < 0) return; // On ne peut pas soigner d'un montant négatif.

        // On ajoute la vie et on s'assure qu'elle ne dépasse pas le maximum.
        _currentHealth = Mathf.Min(_currentHealth + healAmount, _maxHealth);

        // On met à jour l'UI.
        OnHealthChanged?.Invoke(_currentHealth);
        
        Debug.Log($"Player healed for {healAmount}. Current health: {_currentHealth}");
    }

    private void Die()
    {
        Debug.Log("Player has died.");
        OnPlayerDied?.Invoke();

        // Ici, vous pouvez ajouter la logique de mort :
        // - Désactiver le contrôle du joueur
        // - Lancer une animation de mort
        // - Afficher un écran de "Game Over"
        
        // Pour l'instant, on désactive simplement le GameObject.
        gameObject.SetActive(false);
    }

    // --- Pour les tests ---
    // Permet de tester la prise de dégâts depuis l'inspecteur en mode jeu.
    [ContextMenu("Test Take 10 Damage")]
    private void TestDamage()
    {
        TakeDamage(10);
    }
}
